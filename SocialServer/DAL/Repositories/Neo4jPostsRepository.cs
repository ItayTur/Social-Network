﻿using Common.Dtos;
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
        public async Task Add(string posterId, PostModel post, IEnumerable<TagDto> tags)
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
        /// Gets the posts tagging the user associated with the specified id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostModel>> GetTaggingUserPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(post:Post)-[:TAG]-(taggedUser:User)")
                    .Where((UserModel taggedUser) => taggedUser.Id == userId)
                    .Return(post => post.As<PostModel>())
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
        /// Gets the posts the user associated with the id specified is tagged on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostModel>> GetUserTaggedInCommentPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(post:Post)<-[:ON]-(comment:Comment)-[:TAG]->(user:User)")
                    .Where((UserModel user) => user.Id == userId)
                    .Return(post => post.As<PostModel>())
                    .OrderByDescending("post.DateTime")
                    .Limit(postsToShow)
                    .ResultsAsync;
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Gets the posts that's been published by the users the specified user follow. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostModel>> GetFollowedUsersPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(recievingUser: User)-[:FOLLOW]->(postingUser: User)-[:POST]->(post: Post)")
                     .Where((UserModel recievingUser) => recievingUser.Id == userId)
                     .Return(post => post.As<PostModel>())
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
        public async Task<IEnumerable<PostModel>> GetPublicPosts(string userId, int postsToShow)
        {
            try
            {
                return await _graphClient.Cypher.Match("(poster: User)-[:POST]->(post: Post), (receivingUser:User)")
                    .Where((UserModel receivingUser) => receivingUser.Id == userId)
                    .AndWhere((PostModel post) => post.IsPublic == true)
                    .AndWhere("not (receivingUser)-[:FOLLOW]->(poster) and receivingUser<>poster")
                    .Return(post => post.As<PostModel>())
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
    }
}
