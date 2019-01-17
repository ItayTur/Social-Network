using BL.Helpers;
using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Common.Loggers;
using Common.Models;
using Facebook;
using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Data.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly IAuthRepository _authRepository;
        private readonly ILoginTokenManager _loginTokenManager;
        private readonly string _socialUrl;
        private readonly string _identityUrl;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="authRepository"></param>
        /// <param name="loginTokenManager"></param>
        public AuthManager(IAuthRepository authRepository, ILoginTokenManager loginTokenManager)
        {
            _authRepository = authRepository;
            _loginTokenManager = loginTokenManager;
            _identityUrl = ConfigurationManager.AppSettings["IdentityUrl"];
            _socialUrl = ConfigurationManager.AppSettings["SocialUrl"];
        }


        /// <summary>
        /// Blocks a user from entering the app.
        /// </summary>
        /// <param name="email"></param>
        public void BlockUser(string email)
        {
            throw new NotImplementedException();
        }


        /// <summary>
        /// Changes the password in case of user forget.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="currentPassword"></param>
        /// <param name="oldPassword"></param>
        /// <returns>If the password did changes, false otherwise.</returns>
        public bool ChangePassword(string accessToken, string currentPassword, string oldPassword)
        {
            throw new NotImplementedException();
        }
                 

        
        /// <summary>
        /// Logins the user associated with the specified email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public async Task<string> LoginUser(string email, string password)
        {
            try
            {
                var auth = _authRepository.GetAuthByEmail(email);
                VerifyAuthPassword(auth, password);
                return await _loginTokenManager.Add(auth.UserId, LoginTokenModel.LoginTypes.UserPassword);
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw ex;
            }
        }



        /// <summary>
        /// Registers the email and the password to the auth table.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>The access token associated with the specified email and password.</returns>
        public async Task<string> RegisterUserAndLogin(RegistrationDto registrationDto)
        {
            try
            {
                VerifyEmailIsUnique(registrationDto.Email);
                string userId = GenerateUserId();
                var appToken = await _loginTokenManager.Add(userId, LoginTokenModel.LoginTypes.UserPassword);
                await AddUserToDatabases(registrationDto, userId, appToken);
                return appToken;
            }
            catch (DuplicateKeyException ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw;
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw;
            }
        }



        /// <summary>
        /// Addes the user to the Users table and the email to the Auth table.
        /// </summary>
        /// <param name="registrationDto"></param>
        /// <param name="userEmail"></param>
        /// <param name="appToken"></param>
        private async Task AddUserToDatabases(RegistrationDto registrationDto, string userId, string appToken)
        {
            try
            {
                Task addUserTask = AddUserToUsersDb(appToken, registrationDto, userId);
                Task addAuthTask = AddUserToAuthDb(registrationDto.Email, SecurePasswordHasher.Hash(registrationDto.Password), userId);
                Task addUserNodeTask = AddUserToGraphDb(appToken, registrationDto.Email);
                Task.WaitAll(addUserTask, addAuthTask, addUserNodeTask);
            }
            catch (AggregateException ae)
            {
                bool isAddUserFail = false, isAddAuthFail = false, isAddToGraphFail = false;
                foreach (var exception in ae.InnerExceptions)
                {
                    if (exception is AddAuthToDbException)
                    {
                        isAddAuthFail = true;
                    }
                    if (exception is AddUserToDbException)
                    {
                        isAddUserFail = true;
                    }
                    if (exception is AddUserToGraphException)
                    {
                        isAddToGraphFail = true;
                    }
                }
                await RollbackSuccededTask(isAddAuthFail, isAddUserFail, isAddToGraphFail, appToken, registrationDto.Email);
                throw new Exception("Internal server error");
            }

        }



        /// <summary>
        /// Addes user to graph database.
        /// </summary>
        /// <param name="appToken"></param>
        /// <param name="facebookUserDto"></param>
        /// <returns></returns>
        private async Task AddUserToGraphDb(string appToken, string email)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var dataToSend = new JObject
                    {
                        { "token", JToken.FromObject(appToken) },
                        { "email", JToken.FromObject(email) }
                    };
                    var response = await httpClient.PostAsJsonAsync(_socialUrl + "Users/AddUser", dataToSend).ConfigureAwait(continueOnCapturedContext: false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new AddUserToGraphException();
                    }
                }
            }
            catch (AddUserToGraphException e)
            {

                throw e;
            }
            catch (Exception e)
            {
                throw new AddUserToGraphException(e.Message);
            }

        }



        /// <summary>
        /// If only one of the specified tasks failed a rollback is preformed on the other.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        private async Task RollbackSuccededTask(bool isAddAuthFail, bool isAddUserFail, bool isAddToGraphFail, string token, string email)
        {
            if (isAddAuthFail && !isAddUserFail && !isAddToGraphFail)
            {
                await RemoveIdentity(token);
                await RemoveGraphNode(token);
            }
            else if (!isAddAuthFail && isAddUserFail && !isAddToGraphFail)
            {
                await RemoveAuthFromDb(email);
                await RemoveGraphNode(token);

            }
            else if (!isAddAuthFail && !isAddUserFail && isAddToGraphFail)
            {
                await RemoveAuthFromDb(email);
                await RemoveIdentity(token);

            }

            else
            {
                await RollbackOnTwoAdditionsFail(isAddAuthFail, isAddUserFail, isAddToGraphFail, token, email);
            }
        }



        /// <summary>
        /// Rollbacks when two database additions failed.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        /// <param name="isAddToGraghFail"></param>
        /// <param name="token"></param>
        /// <param name="facebookId"></param>
        /// <returns></returns>
        private async Task RollbackOnTwoAdditionsFail(bool isAddAuthFail, bool isAddUserFail, bool isAddToGraghFail, string token, string email)
        {
            if (!isAddAuthFail && isAddUserFail && isAddToGraghFail)
            {
                await RemoveAuthFromDb(email);
            }
            else if (isAddAuthFail && !isAddUserFail && isAddToGraghFail)
            {
                await RemoveIdentity(token);
            }
            else if (isAddAuthFail && isAddUserFail && !isAddToGraghFail)
            {
                await RemoveGraphNode(token);
            }
        }



        /// <summary>
        /// Removes the user associated with the specified token from the database.
        /// </summary>
        /// <param name="token"></param>
        private async Task RemoveIdentity(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(_identityUrl + $"/DeleteUser/{token}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Identity server could not remove the user");
                }
            }
        }




        /// <summary>
        /// Removes the user associated with the specified id from the graph DB.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task RemoveGraphNode(string token)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.DeleteAsync(_socialUrl + $"users/DeleteUserByToken/{token}");
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Error with removing from the graph");
                    }
                }
            }
            catch (Exception e)
            {

                throw e;
            }

        }



        /// <summary>
        /// Removes the user associated with the specified email from the database.
        /// </summary>
        /// <param name="userId"></param>
        private async Task RemoveUserFromDb(string userId)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(_identityUrl + $"/{userId}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Identity server could not remove the user");
                }
            }
        }



        /// <summary>
        /// Removes the auth associated with the specified email from the database.
        /// </summary>
        /// <param name="email"></param>
        private async Task RemoveAuthFromDb(string email)
        {
            await _authRepository.Delete(email);
        }



        /// <summary>
        /// Addes mail to the auth table in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private async Task AddUserToAuthDb(string email, string password, string userId)
        {
            try
            {
                await _authRepository.Add(new AuthModel(email, password, userId)).ConfigureAwait(continueOnCapturedContext: false);
            }
            catch (Exception e)
            {
                //TODO: log
                throw new AddAuthToDbException(e.Message);
            }
        }



        /// <summary>
        /// Adds a user entity to the users database through the identity service.
        /// </summary>
        /// <param name="appToken"></param>
        /// <param name="user"></param>
        private async Task AddUserToUsersDb(string appToken, RegistrationDto registrationDto, string userId)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var user = new UserModel()
                    {
                        Id = userId,
                        FirstName = registrationDto.FirstName,
                        LastName = registrationDto.LastName,
                        Email = registrationDto.Email,
                        Address = registrationDto.Address,
                        Job = registrationDto.Job,
                        BirthDate = registrationDto.BirthDate                        
                    };
                    var data = new JObject();
                    data.Add("user", JToken.FromObject(user));
                    data.Add("token", JToken.FromObject(appToken));
                    var response = await httpClient.PostAsJsonAsync(_identityUrl, data).ConfigureAwait(continueOnCapturedContext: false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new Exception("Identity server could not add the user");
                    }
                }
            }
            catch (Exception e)
            {
                //TODO: log
                throw new AddUserToDbException(e.Message);
            }

        }




        /// <summary>
        /// Verfies the email occupation. Throws an exception other wise.
        /// </summary>
        /// <param name="email"></param>
        private void VerifyEmailIsUnique(string email)
        {
            if (!_authRepository.IsEmailFree(email))
            {
                throw new DuplicateKeyException(email, "Email already exists");
            }
        }



        
        /// <summary>
        /// Verfies the auth password. Throws an exception if not valid.
        /// </summary>
        /// <param name="auth"></param>
        /// <param name="password"></param>
        private void VerifyAuthPassword(AuthModel auth, string password)
        {
            if (auth == null || !SecurePasswordHasher.Verify(password, auth.Password))
            {
                throw new ArgumentException();
            }
        }



        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
