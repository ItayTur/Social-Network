using BL.Helpers;
using Common.Dtos;
using Common.Exceptions;
using Common.Interfaces;
using Common.Loggers;
using Common.Models;
using Facebook;
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
        /// Signs in the user through facebook api.
        /// </summary>
        /// <param name="facebookToken"></param>
        /// <returns>The app token</returns>
        public async Task<string> FacebookSignIn(string facebookToken)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                HttpResponseMessage response = GetUserByFacebookToken(facebookToken, httpClient);
                if (response.IsSuccessStatusCode)
                {
                    var facebookUserDto = await response.Content.ReadAsAsync<FacebookUserDto>();
                    var userEmail = facebookUserDto.email;
                    var appToken = _loginTokenManager.Add(userEmail);
                    if (_authRepository.IsEmailFree(userEmail))
                    {
                        await AddUserToDatabase(facebookUserDto, userEmail, appToken);
                    }
                    return appToken;

                }
                else
                {
                    throw new ArgumentException("Access token is not valid");
                }
            }
        }



        /// <summary>
        /// Addes the user to the Users table and the email to the Auth table.
        /// </summary>
        /// <param name="facebookUserDto"></param>
        /// <param name="userEmail"></param>
        /// <param name="appToken"></param>
        private async Task AddUserToDatabase(FacebookUserDto facebookUserDto, string userEmail, string appToken)
        {
            try
            {
                Task addUserTask = AddUserToUsersDb(appToken, facebookUserDto);
                Task addAuthTask = AddUserToAuthDb(userEmail);
                Task.WaitAll(addUserTask, addAuthTask);
            }
            catch (AggregateException ae)
            {
                bool isAddUserFail = false, isAddAuthFail= false;
                foreach (var exception in ae.InnerExceptions)
                {
                    if(exception is AddAuthToDbException)
                    {
                        isAddAuthFail = true;
                    }
                    if(exception is AddUserToDbException)
                    {
                        isAddUserFail = true;
                    }
                }
                await RollbackSuccededTask(isAddAuthFail, isAddUserFail, userEmail);
                throw new Exception();
            }
            
        }



        /// <summary>
        /// If only one of the specified tasks failed a rollback is preformed on the other.
        /// </summary>
        /// <param name="isAddAuthFail"></param>
        /// <param name="isAddUserFail"></param>
        private async Task RollbackSuccededTask(bool isAddAuthFail, bool isAddUserFail, string userEmail)
        {
            if (isAddAuthFail && !isAddUserFail)
            {
                await RemoveUserFromDb(userEmail);
            }
            else if(!isAddAuthFail && isAddUserFail)
            {
                RemoveAuthFromDb(userEmail);
            }
        }
        


        /// <summary>
        /// Removes the user associated with the specified email from the database.
        /// </summary>
        /// <param name="userEmail"></param>
        private async Task RemoveUserFromDb(string userEmail)
        {
            using(HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.DeleteAsync(_identityUrl+$"/{userEmail}");
                if (!response.IsSuccessStatusCode)
                {
                    throw new ArgumentException("Identity server could not remove the user");
                }
            }
        }



        /// <summary>
        /// Removes the auth associated with the specified email from the database.
        /// </summary>
        /// <param name="userEmail"></param>
        private void RemoveAuthFromDb(string userEmail)
        {
            _authRepository.Delete(userEmail);
        }



        /// <summary>
        /// Addes mail to the auth table in the database.
        /// </summary>
        /// <param name="userEmail"></param>
        /// <returns></returns>
        private Task AddUserToAuthDb(string userEmail)
        {
            try
            {
                return Task.Run(() => _authRepository.Add(new AuthModel(userEmail)));
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
        private async Task AddUserToUsersDb(string appToken, FacebookUserDto user)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {

                    var response = await httpClient.PostAsJsonAsync(_identityUrl, user).ConfigureAwait(continueOnCapturedContext: false);
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
        /// Gets the user details associated with the specified facebook token.
        /// </summary>
        /// <param name="facebookToken"></param>
        /// <param name="httpClient"></param>
        /// <returns>An HttpResponseMessage with the user details or the reason why it failed.</returns>
        private HttpResponseMessage GetUserByFacebookToken(string facebookToken, HttpClient httpClient)
        {
            
           httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", facebookToken);
            httpClient.BaseAddress = new Uri("https://graph.facebook.com/v3.2/");
            httpClient.DefaultRequestHeaders
            .Accept
            .Add(new MediaTypeWithQualityHeaderValue("application/json"));
            return httpClient.GetAsync($"me?fields=id,name,email,first_name,last_name").Result;
        }


        
        /// <summary>
        /// Logins the user associated with the specified email and password.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string LoginUserByUserPassword(string email, string password)
        {
            try
            {
                var auth = _authRepository.GetAuthByEmail(email);
                VerifyAuthPassword(auth, password);
                return _loginTokenManager.Add(email);
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
        public string RegisterUserByUsernamePasswordAndLogin(string email, string password)
        {
            try
            {
                VerifyEmailIsFree(email);
                AuthModel auth = new AuthModel(email, SecurePasswordHasher.Hash(password));
                _authRepository.Add(auth);
                return _loginTokenManager.Add(email);
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
    }
}
