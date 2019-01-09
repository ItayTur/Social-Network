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
        public async Task Add(int posterId, PostModel post, params string[] tagIds)
        {
            _graphClient.Cypher.Merge($"(postingUser:UserModel{{Id: {posterId}}})")
                 .Merge($"(post:PostModel {{Content: {post.Content}, ImgUrl: {post.ImgUrl},DateTime: {post.DateTime}, Likes: {0}}})")
                 .Merge($"(postingUser)-[:POSTED]->(post)").ExecuteWithoutResults();
            foreach (var tagId in tagIds)
            {
                await _graphClient.Cypher.Merge($"(taggedUser: UserModel{{Id: {tagId}}})").ExecuteWithoutResultsAsync();
            }

        }
    }
}
