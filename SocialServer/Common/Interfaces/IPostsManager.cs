using Common.Dtos;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace Common.Interfaces
{
    public interface IPostsManager
    {
        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        Task Add( HttpRequest httpRequest, string token, string path);


        /// <summary>
        /// Searches the text specified and return the matching results.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        Task<IEnumerable<UserModel>> SearchTag(string text, string id);

        /// <summary>
        /// Gets posts to user. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<PostModel>> GetPosts(string token);

        /// <summary>
        /// Adds like to the post associated with id extracted from the http request.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task LikePost(string token, HttpRequest httpRequest);

        /// <summary>
        /// Checks if the user associated with specified user id 
        /// liked the post associated with the specified post id.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsPostLiked(string token, HttpRequest httpRequest);
    }
}
