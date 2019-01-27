using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        /// Changes the user password.
        /// </summary>
        /// <param name="accessToken"></param>
        /// <param name="currentPassword"></param>
        /// <param name="oldPassword"></param>
        /// <returns></returns>
        Task ChangePassword(string token, string currentPassword, string oldPassword);
        

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
    }
}
