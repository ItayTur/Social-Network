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
        Task<UserModel> Get(string id, string token);

        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        Task Add(UserModel user, string token);

        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        Task Delete(string id, string token);
    }
}
