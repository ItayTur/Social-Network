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
       

        bool ChangePassword(string accessToken, string currentPassword, string oldPassword);

        void BlockUser(string email);

        /// <summary>
        /// Registers and logs in the user by email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<string> RegisterUserAndLogin(RegistrationDto registrationDto);
    }
}
