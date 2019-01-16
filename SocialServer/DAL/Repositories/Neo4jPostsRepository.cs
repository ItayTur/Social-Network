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
    public class Neo4jPostsRepository : IPostsRepository
    {
        private readonly GraphClient _graphClient;

        /// <summary>
        /// Constructor.
        /// </summary>
        public Neo4jPostsRepository()
        {
            string url = ConfigurationManager.AppSettings["Neo4jUrl"];
            string username = ConfigurationManager.AppSettings["Username"];
            string password = ConfigurationManager.AppSettings["Password"];

            _graphClient = new GraphClient(new Uri(url), username, password);
            _graphClient.Connect();
        }


        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        public async Task Add(string posterId, PostModel post, IEnumerable<TagDto>tags)
        {
            try
            {
                await _graphClient.Cypher.Match("(postingUser: User)")
                    .Where((UserModel postingUser) => postingUser.Id == posterId)
                    .Create("(postingUser)-[:POST]->(post:Post {post})")
                    .WithParam("post", post)
                    .ExecuteWithoutResultsAsync().ConfigureAwait(false);
                
                foreach (var tag in tags)
                {
                    await _graphClient.Cypher.Match("(taggingPost: Post)", "(taggedUser: User)")
                        .Where((PostModel taggingPost) => taggingPost.Id == post.Id)
                        .AndWhere((UserModel taggedUser) => taggedUser.Id == tag.Id)
                        .CreateUnique("(taggingPost)-[:TAG]->(taggedUser)")
                        .ExecuteWithoutResultsAsync();
                }
            }
            catch (Exception e)
            {
                
                throw new Exception(e.Message);
            }
           

        }


        /// <summary>
        /// Gets the emailes containing the text specified.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> GetUsersByEmailText(string taggerId,string text)
        {
            try
            {
                return await _graphClient.Cypher.Match("(user:User)")
                .Where((UserModel user) => user.Email.Contains(text) && user.Id!=taggerId)
                .Return(user => user.As<UserModel>())
                .ResultsAsync;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
        }
    }
}
