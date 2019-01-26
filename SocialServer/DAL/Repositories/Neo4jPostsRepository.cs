using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using Neo4jClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
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
        public async Task<PostModel> Add(string posterId, PostModel post, IEnumerable<TagDto> tags)
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
                return post;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }


        }




        /// <summary>
        /// Gets the posts tagging the user associated with the specified id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostWithTagsDto>> GetTaggingUserPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u3:User)-[:POST]->(p1:Post)-[:TAG]->(u1:User)")
                   .Where((UserModel u1) => u1.Id == userId)
                   .AndWhere("not (u3)-[:BLOCK]-(u1)")
                   .OptionalMatch("(p1)-[:TAG]->(u2:User)")
                   .Return((p1, u2) => new PostWithTagsDto { Post = p1.As<PostModel>(), Tags = u2.CollectAs<UserModel>() })
                   .OrderByDescending("p1.DateTime")
                   .Limit(postsToShow)
                   .ResultsAsync;

            }
            catch (Exception e)
            {

                throw;
            }


        }



        /// <summary>
        /// Gets the posts the user associated with the id specified is tagged on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostWithTagsDto>> GetUserTaggedInCommentPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(poster:User)-[:POST]->(post:Post)<-[:ON]-(comment:Comment)-[:TAG]->(user:User)")
                    .Where((UserModel user) => user.Id == userId)
                    .AndWhere("not (poster)-[:BLOCK]-(user)")
                    .OptionalMatch("(post)-[:TAG]->(tag:User)")
                    .Return((post, tag) => new PostWithTagsDto { Post = post.As<PostModel>(), Tags = tag.CollectAs<UserModel>() })
                    .OrderByDescending("post.DateTime")
                    .Limit(postsToShow)
                    .ResultsAsync;
            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the posts that's been published by the users the specified user follow. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostWithTagsDto>> GetFollowedUsersPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(recievingUser: User)-[:FOLLOW]->(postingUser: User)-[:POST]->(post: Post)")
                     .Where((UserModel recievingUser) => recievingUser.Id == userId)
                     .OptionalMatch("(post:Post)-[:TAG]->(tagged:User)")
                     .Return((post, tagged) => new PostWithTagsDto { Post = post.As<PostModel>(), Tags = tagged.CollectAsDistinct<UserModel>() })
                     .OrderByDescending("post.DateTime")
                     .Limit(postsToShow)
                     .ResultsAsync;

            }
            catch (Exception e)
            {

                throw;
            }


        }



        /// <summary>
        /// Gets posts from the users, the user specified not follow and marked as public.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsAmountLeft"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostWithTagsDto>> GetPublicPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(poster:User)-[:POST]->(post:Post{IsPublic: true}), (user:User)")
                     .Where((UserModel user) => user.Id == userId)
                     .AndWhere("not (user)-[:BLOCK]-(poster)")
                     .OptionalMatch("(post)-[:TAG]->(tag:User)")
                     .Return((post, tag) => new PostWithTagsDto { Post = post.As<PostModel>(), Tags = tag.CollectAsDistinct<UserModel>() })
                     .OrderByDescending("post.DateTime")
                     .Limit(postsToShow)
                     .ResultsAsync;
            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the emailes containing the text specified.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> GetUsersByEmailText(string taggerId, string text)
        {
            try
            {
                return await _graphClient.Cypher.Match("(user:User)")
                .Where((UserModel user) => user.Email.Contains(text) && user.Id != taggerId)
                .Return(user => user.As<UserModel>())
                .ResultsAsync;
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }



        /// <summary>
        /// Gets the post associated with the specified id.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<PostModel> GetPostById(string postId)
        {
            try
            {
                var postsFound = await _graphClient.Cypher.Match("(post:Post)")
                    .Where((PostModel post) => post.Id == postId)
                    .Return(post => post.As<PostModel>())
                    .ResultsAsync;
                return postsFound.Single();
            }
            catch (Exception e)
            {

                throw;
            }

        }



        /// <summary>
        /// Update the post associated with the id extracted from the instance specified.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task UpdatePost(PostModel updatedPost)
        {
            try
            {
                await _graphClient.Cypher.Match("(post:Post)")
                   .Where((PostModel post) => post.Id == updatedPost.Id)
                   .Set("post = {updatedPost}")
                   .WithParam("updatedPost", updatedPost)
                   .ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Creates like connection between the post associated with the post id
        /// and the user associated with user id.
        /// </summary>
        /// <param name="likedPost"></param>
        /// <returns></returns>
        public async Task LikePost(string postId, string userId)
        {
            try
            {
                await _graphClient.Cypher.Match("(likedPost: Post), (user:User)")
               .Where((UserModel user) => user.Id == userId)
               .AndWhere((PostModel likedPost) => likedPost.Id == postId)
               .Merge("(likedPost)<-[:LIKE]-(user)")
               .OnCreate()
               .Set("likedPost.Likes = likedPost.Likes+1")
               .ExecuteWithoutResultsAsync();


            }
            catch (Exception e)
            {

                throw;
            }


        }



        /// <summary>
        /// Checks if the user associated with specified user id 
        /// liked the post associated with the specified post id.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> IsPostLikedBy(string userId, string postId)
        {
            try
            {
                var boolsSearched = await _graphClient.Cypher.Match("(user:User), (post:Post)")
                    .Where((UserModel user) => user.Id == userId)
                    .AndWhere((PostModel post) => post.Id == postId)
                    .Return<bool>("EXISTS ((user)-[:LIKE]->(post))")
                    .ResultsAsync;
                return boolsSearched.Single();

            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Deletes like connection between the post associated with the specified post id
        /// and the user associated with the specified user id 
        /// </summary>
        /// <param name="likedPost"></param>
        /// <returns></returns>
        public async Task UnLikePost(string userId, string postId)
        {
            try
            {
                await _graphClient.Cypher.Match("(likedPost: Post)<-[like:LIKE]-(user:User)")
               .Where((UserModel user) => user.Id == userId)
               .AndWhere((PostModel likedPost) => likedPost.Id == postId)
               .Set("likedPost.Likes = likedPost.Likes-1")
               .Delete("like")
               .ExecuteWithoutResultsAsync();
            }
            catch (Exception e)
            {

                throw;
            }
        }




        /// <summary>
        /// Addes comment to the post associated with the specified post id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="comment"></param>
        /// <param name="taggedIds"></param>
        /// <returns></returns>
        public async Task<CommentModel> AddComment(string userId, string postId, CommentModel comment, IEnumerable<TagDto> tags)
        {
            try
            {
                await _graphClient.Cypher.Match("(user:User), (post:Post)")
                .Where((UserModel user) => user.Id == userId)
                .AndWhere((PostModel post) => post.Id == postId)
                .Create("(user)-[:COMMENT]->(comment:Comment {comment})-[:ON]->(post)")
                .WithParam("comment", comment)
                .ExecuteWithoutResultsAsync();

                foreach (var tag in tags)
                {
                    await _graphClient.Cypher.Match("(commentAdded:Comment), (taggedUser:User)")
                        .Where((CommentModel commentAdded) => commentAdded.Id == comment.Id)
                        .AndWhere((UserModel taggedUser) => taggedUser.Id == tag.Id)
                        .Create("(taggedUser)<-[:TAG]-(commentAdded)")
                        .ExecuteWithoutResultsAsync();
                }
                return comment;
            }
            catch (Exception e)
            {

                throw;
            }

        }




        /// <summary>
        /// Gets the comments and their tags, of the post associated with the specified post id.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<CommentAndTaggedUsersDto>> GetCommentsOfPost(string postId, int commentsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(post:Post)")
                    .Where((PostModel post) => post.Id == postId)
                    .With("[(post)<-[:ON]-(comment:Comment)<-[:COMMENT]-(writer:User) | {Comment: comment, TaggedUsers: [(comment)-[:TAG]->(taggedUser:User) | taggedUser], Writer: writer}] as result unwind result as res")
                    .Return((res) => res.As<CommentAndTaggedUsersDto>())
                    .OrderByDescending("res.Comment.DateTime")
                    .Limit(commentsToShow)
                    .ResultsAsync;

            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the post of he user associated with the specified Id.
        /// </summary>
        /// <returns></returns>
        public async Task<IEnumerable<PostWithTagsDto>> GetUserPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(u1:User)-[:POST]->(p:Post)")
                    .Where((UserModel u1)=>u1.Id==userId)
                    .OptionalMatch("(p)-[:TAG]->(u2:User)")
                    .Return((p, u2) => new PostWithTagsDto { Post = p.As<PostModel>(), Tags = u2.CollectAsDistinct<UserModel>() })
                     .OrderByDescending("p.DateTime")
                     .Limit(postsToShow)
                     .ResultsAsync;
            }
            catch (Exception e)
            {

                throw;
            }
        }


        /// <summary>
        /// Gets the user of the post
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        public async Task<UserModel> GetPostUser(string postId)
        {
            try
            {             
                var users = await _graphClient.Cypher.Match("(user:User)-[:POST]->(post:Post)")
                .Where((PostModel post) => post.Id == postId)
                .Return(user => user.As<UserModel>())
                .ResultsAsync;

                return users.Single<UserModel>();
            }
            catch (Exception e)
            {

                throw;
            }
        }
    }
}
