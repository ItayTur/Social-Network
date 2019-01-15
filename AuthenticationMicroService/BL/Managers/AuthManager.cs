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
        public string LoginUser(string email, string password)
        {
            try
            {
                var auth = _authRepository.GetAuthByEmail(email);
                VerifyAuthPassword(auth, password);
                return _loginTokenManager.Add(auth.UserId, LoginTokenModel.LoginTypes.UserPassword);
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
                VerifyEmailIsFree(registrationDto.Email);
                string userId = GenerateUserId();
                var appToken = _loginTokenManager.Add(userId, LoginTokenModel.LoginTypes.UserPassword);
                await AddUserToDatabase(registrationDto, userId, appToken);
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
        private async Task AddUserToDatabase(RegistrationDto registrationDto, string userId, string appToken)
        {
            try
            {
                Task addUserTask = AddUserToUsersDb(appToken, registrationDto, userId);
                Task addAuthTask = AddUserToAuthDb(registrationDto.Email, SecurePasswordHasher.Hash(registrationDto.Password), userId);
                Task.WaitAll(addUserTask, addAuthTask);
            }
            catch (AggregateException ae)
            {
                bool isAddUserFail = false, isAddAuthFail = false;
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
                }
                await RollbackSuccededTask(isAddAuthFail, isAddUserFail, userId, registrationDto.Email);
                throw new Exception("Internal server error");
            }

        }

        /// <summary>
        /// If only one of the specified tasks failed a rollback is preformed on the other.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        private async Task RollbackSuccededTask(bool isAddAuthFail, bool isAddUserFail, string userId, string email)
        {
            if (isAddAuthFail && !isAddUserFail)
            {
                await RemoveUserFromDb(userId);
            }
            else if (!isAddAuthFail && isAddUserFail)
            {
                RemoveAuthFromDb(email);
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
        private void RemoveAuthFromDb(string email)
        {
            _authRepository.Delete(email);
        }



        /// <summary>
        /// Addes mail to the auth table in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private Task AddUserToAuthDb(string email, string password, string userId)
        {
            try
            {
                return Task.Run(() => _authRepository.Add(new AuthModel(email, password, userId)));
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
        private void VerifyEmailIsFree(string email)
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
