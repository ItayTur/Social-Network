using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Interfaces
{
    public interface IAuthManager
    {
        /// <summary>
        /// Login user by username and password credentials
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns>Access token</returns>
        Task<string> LoginUser(string email, string password);
       
        

        /// <summary>
        /// Registers and logs in the user by email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> RegisterUserAndLogin(RegistrationDto registrationDto);

        /// <summary>
        /// Activates the system block on the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="blockedId"></param>
        /// <returns></returns>
        Task BlockUser(string token, string blockedId);

        /// <summary>
        /// Restes the users password.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        Task ResetPassword(string token, HttpRequest httpRequest);
    }
}
