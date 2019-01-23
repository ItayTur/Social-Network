using Common.Dtos;
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
        Task<PostModel> Add(string posterId, PostModel post, IEnumerable<TagDto> tags);


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
        Task<IEnumerable<PostWithTagsDto>> GetFollowedUsersPosts(string userId, int postsToShow);


        /// <summary>
        /// Gets posts from the users, the user specified not follow and marked as public.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsAmountLeft"></param>
        /// <returns></returns>
        Task<IEnumerable<PostWithTagsDto>> GetPublicPosts(string userId, int postsToShow);


        /// <summary>
        /// Gets the posts tagging the user associated with the specified id. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<IEnumerable<PostWithTagsDto>> GetTaggingUserPosts(string userId, int postsToShow);

        /// <summary>
        /// Gets the posts the user associated with the id specified is tagged on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        Task<IEnumerable<PostWithTagsDto>> GetUserTaggedInCommentPosts(string userId, int postsToShow);

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
        /// Creates like connection between the post associated with the post id
        /// and the user associated with user id.
        /// </summary>
        /// <param name="likedPost"></param>
        /// <returns></returns>
        Task LikePost(string postId, string userId);

        /// <summary>
        /// Checks if the user associated with specified user id 
        /// liked the post associated with the specified post id.
        /// </summary>
        /// <returns></returns>
        Task<bool> IsPostLikedBy(string userId, string postId);

        /// <summary>
        /// Deletes like connection between the post associated with the specified post id
        /// and the user associated with the specified user id 
        /// </summary>
        /// <param name="likedPost"></param>
        /// <returns></returns>
        Task UnLikePost(string userId, string postId);


        /// <summary>
        /// Addes comment to the post associated with the specified post id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postId"></param>
        /// <param name="comment"></param>
        /// <param name="taggedIds"></param>
        /// <returns></returns>
        Task<CommentModel> AddComment(string userId, string postId, CommentModel comment, IEnumerable<TagDto> tags);

        /// <summary>
        /// Gets the comments and their tags, of the post associated with the specified post id.
        /// </summary>
        /// <returns></returns>
        Task<IEnumerable<CommentAndTaggedUsersDto>> GetCommentsOfPost(string postId, int commentsToShow);
    }
}
