using Common.Dtos;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

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
        Task Add(string token, string email, string name);

        /// <summary>
        /// Gets all the users except the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        Task<IEnumerable<UserWithRelationsDto>> GetUsers(string token);

        /// <summary>
        /// Creates follow relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followedById"></param>
        /// <returns></returns>
        Task CreateFollow(string token, HttpRequest httpRequest);
    }
}
