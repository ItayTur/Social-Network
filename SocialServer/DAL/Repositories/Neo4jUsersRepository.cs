using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class Neo4jUsersRepository : IUsersRepository
    {
        private readonly GraphClient _graphClient;

        public Neo4jUsersRepository()
        {
            string url = ConfigurationManager.AppSettings["Neo4jUrl"];
            string username = ConfigurationManager.AppSettings["Username"];
            string password = ConfigurationManager.AppSettings["Password"];
            _graphClient = new GraphClient(new Uri(url), username, password);
            _graphClient.Connect();
        }

        /// <summary>
        /// Adds user to the database.
        /// </summary>
        /// <param name="userToAdd"></param>
        /// <returns></returns>
        public async Task Add(UserModel userToAdd)
        {
            try
            {
                await _graphClient.Cypher.Merge("(user:User {Id: {id}})")
                    .OnCreate()
                    .Set("user = {userToAdd}")
                    .WithParams(new
                    {
                        id = userToAdd.Id,
                        userToAdd
                    }).ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
        }



        /// <summary>
        /// Deletes the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="token"></param>
        public async Task Delete(string userId)
        {
            try
            {
                await _graphClient.Cypher.Match("(user: User)")
                .Where((UserModel user) => user.Id == userId)
                .Delete("user")
                .ExecuteWithoutResultsAsync();
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
        public async Task<IEnumerable<UserModel>> GetUsers(string userId, int usersToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u:User)")
                    .Where((UserModel u) => u.Id != userId)
                    .Return(u => u.As<UserModel>())
                    .Limit(usersToShow)
                    .ResultsAsync;
            }
            catch (Exception e)
            {

                throw;
            }
        }


        /// <summary>
        /// Gets the users that's being followed by the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserWithRelationsDto>> GetUserFollowings(string userId, int usersToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u1:User)-[r:FOLLOW]->(u2:User)")
                    .Where((UserModel u1) => u1.Id == userId)
                    .Return((u2,r) => new UserWithRelationsDto { User=u2.As<UserModel>(), IsFollow = r!=null})
                    .Limit(usersToShow)
                    .ResultsAsync;

            }
            catch (Exception e)
            {

                throw;
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
        public async Task<IEnumerable<UserWithRelationsDto>> GetUserUnfollowings(string userId, int usersToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u1:User),(u2:User)")
                    .Where((UserModel u1) => u1.Id == userId)
                    .AndWhere("not (u1)-[:FOLLOW]->(u2) AND not (u1)-[:BLOCK]->(u2) AND u1<>u2")
                    .Return(u2 => new UserWithRelationsDto { User = u2.As<UserModel>() })
                    .Limit(usersToShow)
                    .ResultsAsync;
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
        public async Task<IEnumerable<UserWithRelationsDto>> GetBlockedUsers(string userId, int usersToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u1:User)-[r:BLOCK]->(u2:User)")
                    .Where((UserModel u1) => u1.Id == userId)
                    .Return((u2,r) => new UserWithRelationsDto { User = u2.As<UserModel>(), IsBlock = r!=null })
                    .Limit(usersToShow)
                    .ResultsAsync;

               
            }
            catch (Exception e)
            {

                throw e;
            }
        }


        /// <summary>
        /// Creates follow relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followedById"></param>
        /// <returns></returns>
        public async Task CreateFollow(string followerId, string followedById)
        {
            try
            {
                await _graphClient.Cypher.Match("(userFollow:User), (userFollowedBy:User)")
                    .Where((UserModel userFollow) => userFollow.Id == followerId)
                    .AndWhere((UserModel userFollowedBy) => userFollowedBy.Id == followedById)
                    .CreateUnique("(userFollow)-[:FOLLOW]->(userFollowedBy)")
                    .ExecuteWithoutResultsAsync();
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// Deletes follow relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="followerId"></param>
        /// <param name="followedById"></param>
        /// <returns></returns>
        public async Task DeleteFollow(string followerId, string followedById)
        {
            try
            {
                await _graphClient.Cypher.Match("(userFollow:User)-[r:FOLLOW]->(userFollowedBy:User)")
                    .Where((UserModel userFollow) => userFollow.Id == followerId)
                    .AndWhere((UserModel userFollowedBy) => userFollowedBy.Id == followedById)
                    .Delete("r")
                    .ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Creates block relation between the users associated with the specified ids.
        /// </summary>
        /// <param name="blockerId"></param>
        /// <param name="blockedId"></param>
        /// <returns></returns>
        public async Task CreateBlock(string blockerId, string blockedId)
        {
            try
            {
                await _graphClient.Cypher
                    .Match("(blocker:User), (blocked:User)")
                    .Where((UserModel blocker) => blocker.Id == blockerId)
                    .AndWhere((UserModel blocked) => blocked.Id == blockedId)
                    .OptionalMatch("(blocker)-[r:FOLLOW]->(blocked)")
                    .Create("(blocker)-[:BLOCK]->(blocked)")
                    .Delete("r")
                    .ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
