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
        Task<PostModel> Add( HttpRequest httpRequest, string token, string path);


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
        Task<IEnumerable<PostWithTagsDto>> GetPosts(string token);


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
        Task<bool> IsPostLikedBy(string token, HttpRequest httpRequest);


        /// <summary>
        /// Deletes like from the post associated with id extracted from the http request.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task UnLikePost(string token, HttpRequest httpRequest);


        /// <summary>
        /// Adds comment to the post associated with the post id
        /// extracted from the http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="token"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        Task<CommentModel> AddComment(HttpRequest httpRequest, string token, string path);


        /// <summary>
        /// Gets the comments (with their tags) of the post associated with the post id specified.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<IEnumerable<CommentAndTaggedUsersDto>> GetCommentsOfPost(string postId, string token);
    }
}
