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
    }
}
