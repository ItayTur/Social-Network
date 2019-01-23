using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Security.Authentication;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class UsersManager : IUsersManager
    {
        private delegate Task GetUsersHandler(ICollection<UserWithRelationsDto> usersToReturn, string userId, int usersToShow, HashSet<string> usedIds);
        private readonly GetUsersHandler[] _getUsersHandlers;
        private readonly IUsersRepository _usersRepository;
        private readonly ICommonOperationsManager _commonOperationsManager;

        private readonly string _authBaseUrl;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersRepository"></param>
        public UsersManager(IUsersRepository usersRepository, ICommonOperationsManager commonOperationsManager)
        {
            _getUsersHandlers = new GetUsersHandler[3];
            InitializeHandlers();
            _usersRepository = usersRepository;
            _commonOperationsManager = commonOperationsManager;
            _authBaseUrl = ConfigurationManager.AppSettings["AuthBaseUrl"];
        }


        /// <summary>
        /// Initializes the get users handlers array.
        /// </summary>
        private void InitializeHandlers()
        {
            _getUsersHandlers[0] = GetUserFollowings;
            _getUsersHandlers[1] = GetUserUnfollowings;
            _getUsersHandlers[2] = GetBlockedUsers;
        }



        /// <summary>
        /// Addes user with the email specified.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        /// <returns></returns>
        public async Task Add(string token, string email, string name)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                UserModel userToAdd = CreateUser(email, userId, name);
                await _usersRepository.Add(userToAdd);
            }
            catch (Exception)
            {

                throw;
            }

        }



        /// <summary>
        /// Creates UserModel instance 
        /// associated with the specified email and user ID.
        /// </summary>
        /// <param name="email"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        private UserModel CreateUser(string email, string userId, string name)
        {
            return new UserModel()
            {
                Id = userId,
                Email = email,
                Name = name
            };
        }



        /// <summary>
        /// Deletes the user associated with the specified token.
        /// </summary>
        /// <param name="token"></param>
        public async Task Delete(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                await _usersRepository.Delete(userId);
            }
            catch (AuthenticationException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        /// <summary>
        /// Gets all the users except the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserWithRelationsDto>> GetUsers(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                string usersToShowString = ConfigurationManager.AppSettings["UsersToShow"];
                int usersToShow = _commonOperationsManager.IntegerBiggerThanZero(usersToShowString);
                var usersToReturn = new List<UserWithRelationsDto>();
                HashSet<string> usedIds = new HashSet<string>();
                for (int i = 0; i < _getUsersHandlers.Length && usersToShow > 0; i++)
                {
                    int usersBeforAddition = usersToReturn.Count;
                    await _getUsersHandlers[i](usersToReturn, userId, usersToShow, usedIds);
                    int usersAdded = usersToReturn.Count - usersBeforAddition;
                    usersToShow -= usersAdded;
                }
                return usersToReturn;
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the users that's being followed by the user
        /// associated with the Id extracted from the token.
        /// </summary>
        /// <returns></returns>
        private async Task GetUserFollowings(ICollection<UserWithRelationsDto> usersToReturn, string userId, int usersToShow, HashSet<string>usedIds)
        {
            try
            {
                var userFollowings = await _usersRepository.GetUserFollowings(userId, usersToShow);
                CheckUniqueId(usersToReturn, usedIds, userFollowings);
            }
            catch (Exception e)
            {

                throw e;
            }
        }



        /// <summary>
        /// Adds to the returned collection only user that hasn't been added yet. 
        /// </summary>
        /// <param name="usersToReturn"></param>
        /// <param name="usedIds"></param>
        /// <param name="usersToCheck"></param>
        private void CheckUniqueId(ICollection<UserWithRelationsDto> usersToReturn, HashSet<string> usedIds, IEnumerable<UserWithRelationsDto> usersToCheck)
        {
            foreach (var userDto in usersToCheck)
            {
                var user = userDto.User;
                if (!usedIds.Contains(user.Id))
                {
                    usedIds.Add(user.Id);
                    usersToReturn.Add(userDto);
                }
            }
        }



        /// <summary>
        /// Gets the users that the user associated with the specified Id is not following.
        /// </summary>
        /// <param name="usersToReturn"></param>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <param name="usedIds"></param>
        /// <returns></returns>
        private async Task GetUserUnfollowings(ICollection<UserWithRelationsDto> usersToReturn, string userId, int usersToShow, HashSet<string> usedIds)
        {
            try
            {
                IEnumerable<UserWithRelationsDto> userUnfollowings = await _usersRepository.GetUserUnfollowings(userId, usersToShow);
                CheckUniqueId(usersToReturn, usedIds, userUnfollowings);
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }



        /// <summary>
        /// Gets the users that the user associated with the specified Id blockes.
        /// </summary>
        /// <param name="usersToReturn"></param>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <param name="usedIds"></param>
        /// <returns></returns>
        private async Task GetBlockedUsers(ICollection<UserWithRelationsDto> usersToReturn, string userId, int usersToShow, HashSet<string> usedIds)
        {
            try
            {
                var blockedUsers = await _usersRepository.GetBloackedUsers(userId, usersToShow);
                CheckUniqueId(usersToReturn, usedIds, blockedUsers);
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
