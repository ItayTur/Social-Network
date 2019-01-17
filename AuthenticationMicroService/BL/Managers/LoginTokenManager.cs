using Common.Interfaces;
using Common.Loggers;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using static Common.Models.LoginTokenModel;

namespace BL.Managers
{
    public class LoginTokenManager : ILoginTokenManager
    {
        ILoginTokenRepository _loginTokenRepository;


        public LoginTokenManager(ILoginTokenRepository loginTokenRepository)
        {
            _loginTokenRepository = loginTokenRepository;
        }

        /// <summary>
        /// Add auth.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginType"></param>
        /// <returns>Auth token</returns>
        public async Task<string> Add(string userId, LoginTypes loginType)
        {
            try
            {
                var loginToken = new LoginTokenModel(userId, loginType);
                var savedLoginToken = await _loginTokenRepository.AddLoginToken(loginToken);
                return savedLoginToken.Token;
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw ex;
            }
        }

        /// <summary>
        /// Verifies user token
        /// </summary>
        /// <param name="appToken"></param>
        /// <returns>User id</returns>
        public async Task<string> VerifyAsync(string appToken)
        {
            try
            {
                var loginToken = _loginTokenRepository.GetLoginToken(appToken);
                Verify(loginToken);
                await ExtendTokenExpirationAsync(loginToken);
                return loginToken.UserId;
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw ex;
            }
        }

        private async Task ExtendTokenExpirationAsync(LoginTokenModel loginToken)
        {
            loginToken.ExpiredTime = DateTime.Now.AddMinutes(int.Parse(ConfigurationManager.AppSettings["AccessTokenMinutes"]));
            await _loginTokenRepository.Update(loginToken);
        }

        private void Verify (LoginTokenModel loginToken)
        {
            if (loginToken == null || loginToken.ExpiredTime < DateTime.Now)
            {
                throw new AuthenticationException();
            }
        }
    }
}
