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
                var usersToReturn = await _graphClient.Cypher.Match("(u1:User)-[:FOLLOW]->(u2:User)")
                    .Where((UserModel u1) => u1.Id == userId)
                    .Return(u2 => new UserWithRelationsDto { User=u2.As<UserModel>()})
                    .Limit(usersToShow)
                    .ResultsAsync;

                foreach (var user in usersToReturn)
                {
                    user.IsFollow = true;
                }
                return usersToReturn;


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
                    .AndWhere("not (u1)-[:FOLLOW]->(u2) AND not (u1)-[:BLOCK]->(u2)")
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
        public async Task<IEnumerable<UserWithRelationsDto>> GetBloackedUsers(string userId, int usersToShow)
        {
            try
            {
                var usersToReturn = await _graphClient.Cypher.Match("(u1:User)-[:BLOCK]->(u2:User)")
                    .Where((UserModel u1) => u1.Id == userId)
                    .Return(u2 => new UserWithRelationsDto { User = u2.As<UserModel>() })
                    .Limit(usersToShow)
                    .ResultsAsync;

                foreach (var user in usersToReturn)
                {
                    user.IsBlock = true;
                }

                return usersToReturn;
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
