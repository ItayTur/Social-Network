using Common.Interfaces;
using Common.Interfaces.Helpers;
using System;
using BL.Helpers;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Loggers;
using Common.Models;
using System.Data.Linq;

namespace BL.Managers
{
    public class AuthManager : IAuthManager
    {
        IAuthRepository _authRepository;
        ILoginTokenManager _loginTokenManager;

        public AuthManager(IAuthRepository authRepository, ILoginTokenManager loginTokenManager)
        {
            _authRepository = authRepository;
            _loginTokenManager = loginTokenManager;
        }

        public void BlockUser(string email)
        {
            throw new NotImplementedException();
        }

        public bool ChangePassword(string accessToken, string currentPassword, string oldPassword)
        {
            throw new NotImplementedException();
        }

        public string LoginUserByFacebook(string token)
        {
            throw new NotImplementedException();
        }

        public string LoginUserByUserPassword(string email, string password)
        {
            try
            {
                var auth = _authRepository.GetAuthByEmail(email);
                VerifyPasswordVsAuth(auth, password);
                return _loginTokenManager.Add(email);
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw ex;
            }
        }

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

        private void VerifyEmailIsFree(string email)
        {
            if (!_authRepository.IsEmailFree(email))
            {
                throw new DuplicateKeyException(email,"Email already exists");
            }
        }

        private void VerifyPasswordVsAuth(AuthModel auth, string password)
        {
            if (auth == null || !SecurePasswordHasher.Verify(password, auth.Password))
            {
                throw new ArgumentException();
            }
        }
    }
}
