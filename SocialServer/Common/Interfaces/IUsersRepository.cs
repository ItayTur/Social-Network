using Common.Dtos;
using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

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


        /// <summary>
        /// Gets the users that's being followed by the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        Task<IEnumerable<UserWithRelationsDto>> GetUserFollowings(string userId, int usersToShow);


        /// <summary>
        /// Gets the users that the user associated with the specified Id is not following.
        /// </summary>
        /// <param name="usersToReturn"></param>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <param name="usedIds"></param>
        /// <returns></returns>
        Task<IEnumerable<UserWithRelationsDto>> GetUserUnfollowings(string userId, int usersToShow);

        
        /// <summary>
        /// Gets the users that the user associated with the specified Id blockes.
        /// </summary>
        /// <param name="usersToReturn"></param>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <param name="usedIds"></param>
        /// <returns></returns>
        Task<IEnumerable<UserWithRelationsDto>> GetBlockedUsers(string userId, int usersToShow);


        /// <summary>
        /// Creates follow relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followedById"></param>
        /// <returns></returns>
        Task CreateFollow(string followerId, string followedById);


        /// <summary>
        /// Deletes follow relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followedById"></param>
        /// <returns></returns>
        Task DeleteFollow(string followerId, string followedById);

        /// <summary>
        /// Creates block relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="blockerId"></param>
        /// <param name="blockedId"></param>
        /// <returns></returns>
        Task CreateBlock(string blockerId, string blockedId);
    }
}
