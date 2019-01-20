﻿using Common.Dtos;
using Common.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IPostsRepository
    {
        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        Task Add(string posterId, PostModel post, IEnumerable<TagDto> tags);


        /// <summary>
        /// Gets the users containing the text specified in their email.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> GetUsersByEmailText(string taggerId, string text);


        /// <summary>
        /// Gets the posts that's been published by the users the specified user follow. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<PostModel>> GetFollowedUsersPosts(string userId, int postsToShow);


        /// <summary>
        /// Gets posts from the users, the user specified not follow and marked as public.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsAmountLeft"></param>
        /// <returns></returns>
        Task<IEnumerable<PostModel>> GetPublicPosts(string userId, int postsToShow);


        /// <summary>
        /// Gets the posts tagging the user associated with the specified id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<PostModel>> GetTaggingUserPosts(string userId, int postsToShow);

        /// <summary>
        /// Gets the posts the user associated with the id specified is tagged on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        Task<IEnumerable<PostModel>> GetUserTaggedInCommentPosts(string userId, int postsToShow);

        /// <summary>
        /// Gets the post associated with the specified id.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        Task<PostModel> GetPostById(string postId);

        /// <summary>
        /// Update the post associated with the id extracted from the instance specified.
        /// </summary>
        /// <param name="postId"></param>
        /// <returns></returns>
        Task UpdatePost(PostModel updatedPost);

        /// <summary>
        /// Creates like connection between the post specified 
        /// </summary>
        /// <param name="likedPost"></param>
        /// <returns></returns>
        Task<PostModel> LikePost(string postId, string userId);
    }
}
