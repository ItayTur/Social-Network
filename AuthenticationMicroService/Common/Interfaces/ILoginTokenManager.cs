using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Models.LoginTokenModel;

namespace Common.Interfaces
{
    public interface ILoginTokenManager
    {
        /// <summary>
        /// Add auth.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="loginType"></param>
        /// <returns>Auth token</returns>
        Task<string> Add(string userId, LoginTypes loginType);

        /// <summary>
        /// Verifies user token
        /// </summary>
        /// <param name="appToken"></param>
        /// <returns>User id</returns>
        Task<string> VerifyAsync(string appToken);
    }
}
