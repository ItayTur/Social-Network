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
        public string RegisterUserAndLogin(string email, string password)
        {
            try
            {
                VerifyEmailIsFree(email);
                string userId = GenerateUserId();
                AuthModel auth = new AuthModel(email, SecurePasswordHasher.Hash(password), userId);
                _authRepository.Add(auth);
                return _loginTokenManager.Add(userId, LoginTokenModel.LoginTypes.UserPassword);
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

        private string GenerateUserId()
        {
            return Guid.NewGuid().ToString();
        }
    }
}
