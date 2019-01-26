using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUsersManager
    {
        /// <summary>
        /// Gets a user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns>User</returns>
        Task<UserModel> Get(string token);

        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        Task Add(UserModel user, string token);

        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        Task Delete(string token);


        /// <summary>
        /// Gets the full name of the user
        /// associated with id extracted from the token.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetFullName(string token);


        /// <summary>
        /// Gets the email of the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<string> GetUserEmailById(string token, string userId);

        /// <summary>
        /// Get the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<UserModel> GetUserById(string token, string userId);
    }
}
