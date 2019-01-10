using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IFacebookAuthManager
    {
        /// <summary>
        /// Login user by facebook id (acquired through facebook's user access token).
        /// Auth will be created if doesn't exist.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Access token</returns>
        Task<string> SignIn(string facebookToken);       
    }
}
