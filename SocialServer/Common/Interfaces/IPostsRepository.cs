using Common.Dtos;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        
    }
}
