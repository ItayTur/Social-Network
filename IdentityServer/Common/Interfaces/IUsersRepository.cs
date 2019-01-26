using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Gets a user record from the db.
        /// </summary>
        /// <returns>User</returns>
        Task<UserModel> Get(string id);

        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        Task Add(UserModel user);

        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        Task Delete(string id);


        /// <summary>
        /// Gets the full name of the user associated with id specified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<string> GetFullName(string id);

        
    }
}
