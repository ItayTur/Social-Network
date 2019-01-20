using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;

namespace BL.Managers
{
    public delegate Task GetPostsHandler(int postsToShow, string userId, ICollection<PostModel> postsToReturn, HashSet<string> idsUsed);

    public class PostsManager : IPostsManager
    {
        private readonly GetPostsHandler[] _getPostsHandlers;
        private readonly IPostsRepository _postsRepository;
        private readonly IStorageManager _storageManager;
        private readonly ICommonOperationsManager _commonOperationsManager;
        private readonly string _authBaseUrl;
        private readonly string _identityBaseUrl;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postsRepository"></param>
        /// <param name="storageManager"></param>
        public PostsManager(IPostsRepository postsRepository, IStorageManager storageManager, ICommonOperationsManager commonOperationsManager)
        {
            int numberOfPostsHandler = 4;
            _getPostsHandlers = new GetPostsHandler[numberOfPostsHandler];
            InitializePostsHandlers();
            _postsRepository = postsRepository;
            _storageManager = storageManager;
            _commonOperationsManager = commonOperationsManager;
            _authBaseUrl = ConfigurationManager.AppSettings["AuthBaseUrl"];
            _identityBaseUrl = ConfigurationManager.AppSettings["IdentityBaseUrl"];
        }



        /// <summary>
        /// Initializes the posts handlers array with the handling operartions.
        /// </summary>
        private void InitializePostsHandlers()
        {
            int recommendationLvls;
            if (IntegerBiggerThanZeroValidation(ConfigurationManager.AppSettings["PostsRecommendationLvls"], out recommendationLvls))
            {
                _getPostsHandlers[0] = GetTaggingUserPosts;
                _getPostsHandlers[1] = GetUserTaggedInCommentPosts;
                _getPostsHandlers[2] = GetFollowedPosts;
                _getPostsHandlers[3] = GetPublicPosts;
            }
            else
            {
                throw new ArgumentException("The PostsRecommendationLvls value given in the configuration is not valid");
            }
        }


        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        public async Task Add(HttpRequest httpRequest, string token, string path)
        {
            try
            {
                PostModel post = CreatePost(httpRequest["Content"], httpRequest["IsPublic"]);
                List<TagDto> tags = GetTags(httpRequest);
                post.ImgUrl = await GetImageUrl(httpRequest.Files["Pic"], path);
                var userId = await _commonOperationsManager.VerifyToken(token).ConfigureAwait(false);
                post.Id = GenerateId();
                post.WriterName = await GetFullName(token);
                await _postsRepository.Add(userId, post, tags);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Gets the full name of the user
        /// assoiciated with the id extracted from the token specified. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> GetFullName(string token)
        {
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.GetAsync(_identityBaseUrl + "/GetFullName/" + token);
                    if (response.IsSuccessStatusCode)
                    {
                        var fullName = await response.Content.ReadAsAsync<string>();
                        return fullName;
                    }
                    else
                    {
                        throw new Exception();
                    }
                }
            }
            catch (Exception)
            {

                throw;
            }
        }


        /// <summary>
        /// Sets the image url in the PostModel instance specified.
        /// </summary>
        /// <param name="picFile"></param>
        /// <param name="path"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        private async Task<string> GetImageUrl(HttpPostedFile picFile, string path)
        {
            string urlToReturn = "";
            if (picFile != null)
            {
                urlToReturn = await _storageManager.AddPicToStorage(picFile, path).ConfigureAwait(false);
            }

            return urlToReturn;
        }



        /// <summary>
        /// Gets the tags associated with the text
        /// specified in the http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private List<TagDto> GetTags(HttpRequest httpRequest)
        {
            List<TagDto> tags = new List<TagDto>();
            if (httpRequest["Tags"] != "undefined")
            {
                tags = JsonConvert.DeserializeObject<List<TagDto>>((httpRequest["Tags"]));
            }

            return tags;
        }



        /// <summary>
        /// Creates post instance from the content specifid.
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        private PostModel CreatePost(string content, string isPublic)
        {
            return new PostModel()
            {
                Content = content,
                IsPublic = isPublic == "true" ? true : false,
                DateTime = DateTime.Now
            };
        }



        /// <summary>
        /// Searches the text specified and return the matching results.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> SearchTag(string text, string token)
        {
            try
            {
                string taggerId = await _commonOperationsManager.VerifyToken(token);
                return await _postsRepository.GetUsersByEmailText(taggerId, text);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }

        }



        /// <summary>
        /// Generates unique string ID.
        /// </summary>
        /// <returns></returns>
        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }



        /// <summary>
        /// Gets the post recommended for the user 
        /// associated with id extracted from the token.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<IEnumerable<PostModel>> GetPosts(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                int postsToShow;
                if (IntegerBiggerThanZeroValidation(ConfigurationManager.AppSettings["PostsToShow"], out postsToShow))
                {
                    HashSet<string> usedIds = new HashSet<string>();
                    List<PostModel> postsToReturn = new List<PostModel>();
                    for (int i = 0; i < _getPostsHandlers.Length && postsToReturn.Count < postsToShow; i++)
                    {
                        await _getPostsHandlers[i].Invoke(postsToShow, userId, postsToReturn, usedIds);
                        postsToShow = postsToShow - postsToReturn.Count;
                    }
                    return postsToReturn;
                }
                else
                {
                    throw new ArgumentException("Posts to show amount is not a valid integer or bigger than zero");
                }

            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the posts that the user 
        /// associated with the id extracted from the token is tagged in.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task GetTaggingUserPosts(int postsToShow, string userId, ICollection<PostModel> postsToReturn, HashSet<string> idsUsed)
        {
            try
            {
                IEnumerable<PostModel> taggingUserPosts = await _postsRepository.GetTaggingUserPosts(userId, postsToShow);
                GetUniquePosts(postsToReturn, idsUsed, taggingUserPosts);
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the post that hasn't been added to the post returning collection.
        /// </summary>
        /// <param name="postsToReturn"></param>
        /// <param name="idsUsed"></param>
        /// <param name="taggingUserPosts"></param>
        private void GetUniquePosts(ICollection<PostModel> postsToReturn, HashSet<string> idsUsed, IEnumerable<PostModel> postsToCheck)
        {
            try
            {
                var postsList = postsToCheck.ToList();
                foreach (var post in postsList)
                {
                    if (!idsUsed.Contains(post.Id))
                    {
                        postsToReturn.Add(post);
                        idsUsed.Add(post.Id);
                    }
                }
            }
            catch (Exception e)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the posts that's been published by the users the specified user follow. 
        /// </summary>
        /// <param name="postsToShow"></param>
        /// <param name="userId"></param>
        /// <param name="postsToReturn"></param>
        /// <param name="idsUsed"></param>
        /// <returns></returns>
        private async Task GetFollowedPosts(int postsToShow, string userId, ICollection<PostModel> postsToReturn, HashSet<string> idsUsed)
        {
            var followedPosts = await _postsRepository.GetFollowedUsersPosts(userId, postsToShow);
            GetUniquePosts(postsToReturn, idsUsed, followedPosts);
        }



        /// <summary>
        /// Gets the posts the user associated with the id specified is tagged on.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        public async Task GetUserTaggedInCommentPosts(int postsToShow, string userId, ICollection<PostModel> postsToReturn, HashSet<string> idsUsed)
        {
            var taggedInCommentsPosts = await _postsRepository.GetUserTaggedInCommentPosts(userId, postsToShow);
            GetUniquePosts(postsToReturn, idsUsed, taggedInCommentsPosts);
        }



        /// <summary>
        /// Gets public posts.
        /// </summary>
        /// <param name="postsToShow"></param>
        /// <param name="userId"></param>
        /// <param name="postsToReturn"></param>
        /// <param name="idsUsed"></param>
        /// <returns></returns>
        private async Task GetPublicPosts(int postsToShow, string userId, ICollection<PostModel> postsToReturn, HashSet<string> idsUsed)
        {
            var publicPosts = await _postsRepository.GetPublicPosts(userId, postsToShow);
            GetUniquePosts(postsToReturn, idsUsed, publicPosts);
        }


        /// <summary>
        /// Verifys the posts to show number extracted from the app-config.
        /// </summary>
        /// <param name="postsToShow"></param>
        /// <returns></returns>
        private static bool IntegerBiggerThanZeroValidation(string stringToVerify, out int num)
        {
            return int.TryParse(stringToVerify, out num) && num > 0;
        }


        /// <summary>
        /// Adds like to the post associated with id extracted from the http request.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task LikePost(string token, HttpRequest httpRequest)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                string postId = httpRequest["PostId"];
                await _postsRepository.LikePost(postId, userId);

            }
            catch (Exception e)
            {

                throw;
            }
        }


        /// <summary>
        /// Deletes like from the post associated with id extracted from the http request.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task UnLikePost(string token, HttpRequest httpRequest)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                string postId = httpRequest["PostId"];
                await _postsRepository.UnLikePost(userId, postId);

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
        public async Task<bool> IsPostLikedBy(string token, HttpRequest httpRequest)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                string postId = httpRequest["PostId"];
                return await _postsRepository.IsPostLikedBy(userId, postId);
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        /// <summary>
        /// Adds comment to the post associated with the post id
        /// extracted from the http request.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <param name="token"></param>
        /// <param name="path"></param>
        /// <returns></returns>
        public async Task AddComment(HttpRequest httpRequest, string token, string path)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                string postId = httpRequest["PostId"];
                List<TagDto> tags = GetTags(httpRequest);
                CommentModel comment = CreateComment(httpRequest["Content"]);
                comment.ImgUrl = await GetImageUrl(httpRequest.Files["Pic"], path);
                await _postsRepository.AddComment(userId, postId, comment, tags);
            }
            catch (Exception)
            {

                throw;
            }
            
        }


        /// <summary>
        /// Creates a CommentModel instance.
        /// </summary>
        /// <param name="v"></param>
        /// <returns></returns>
        private CommentModel CreateComment(string content)
        {
            return new CommentModel
            {
                Content = content
            };
        }
    }
}
