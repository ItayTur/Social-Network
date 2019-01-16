﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUsersManager
    {
        /// <summary>
        /// Deletes the user associated with the specified token.
        /// </summary>
        /// <param name="token"></param>
        Task Delete(string token);

        /// <summary>
        /// Addes user with the email specified.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        Task Add(string token, string email);
    }
}