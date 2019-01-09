using Common.Interfaces;
using Common.Loggers;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class LoginTokenManager : ILoginTokenManager
    {
        ILoginTokenRepository _loginTokenRepository;

        public LoginTokenManager(ILoginTokenRepository loginTokenRepository)
        {
            _loginTokenRepository = loginTokenRepository;
        }

        public string Add(string userId)
        {
            try
            {
                var loginToken = new LoginTokenModel(userId);
                var savedLoginToken = _loginTokenRepository.AddLoginToken(loginToken);
                return savedLoginToken.Token;
            }
            catch (Exception ex)
            {
                LoggerFactory.GetInstance().AllLogger().Log(ex.Message);
                throw ex;
            }
        }
    }
}
