﻿using System;
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
        string LoginUserByUserPassword(string email, string password);

        /// <summary>
        /// Login user by facebook email (acquired through facebook's user access token).
        /// Auth will be created if doesn't exist.
        /// </summary>
        /// <param name="token"></param>
        /// <returns>Access token</returns>
        string LoginUserByFacebook(string token);

        bool ChangePassword(string accessToken, string currentPassword, string oldPassword);

        void BlockUser(string email);

        /// <summary>
        /// Registers and logs in the user by email and password
        /// </summary>
        /// <param name="email"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        string RegisterUserByUsernamePasswordAndLogin(string email, string password);
    }
}
