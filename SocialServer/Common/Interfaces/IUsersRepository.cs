using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Models;

namespace Common.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Deletes the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        Task Delete(string userId);

       /// <summary>
       /// Adds user to the database.
       /// </summary>
       /// <param name="userToAdd"></param>
       /// <returns></returns>
        Task Add(UserModel userToAdd);


        /// <summary>
        /// Gets all the users except the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetUsers(string userId, int usersToShow);
    }
}
