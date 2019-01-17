using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BL.Managers
{
    public class PostsManager : IPostsManager
    {

        private readonly IPostsRepository _postsRepository;
        private readonly IStorageManager _storageManager;
        private readonly ICommonOperationsManager _commonOperationsManager;
        private readonly string _authBaseUrl;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postsRepository"></param>
        /// <param name="storageManager"></param>
        public PostsManager(IPostsRepository postsRepository, IStorageManager storageManager, ICommonOperationsManager commonOperationsManager)
        {
            _postsRepository = postsRepository;
            _storageManager = storageManager;
            _commonOperationsManager = commonOperationsManager;
            _authBaseUrl = ConfigurationManager.AppSettings["AuthBaseUrl"];
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
                await SetImageUrl(httpRequest.Files["Pic"], path, post);
                var userId = await _commonOperationsManager.VerifyToken(token).ConfigureAwait(false);
                post.Id = GenerateId();
                var addPostToDbTask = _postsRepository.Add(userId, post, tags);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }



        /// <summary>
        /// Sets the image url in the PostModel instance specified.
        /// </summary>
        /// <param name="picFile"></param>
        /// <param name="path"></param>
        /// <param name="post"></param>
        /// <returns></returns>
        private async Task SetImageUrl(HttpPostedFile picFile, string path, PostModel post)
        {
            if (picFile != null)
            {
                post.ImgUrl = await _storageManager.AddPicToStorage(picFile, path).ConfigureAwait(false);
            }
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

        public async Task<IEnumerable<PostModel>> GetPosts(string token)
        {
            try
            {
                if (int.TryParse(ConfigurationManager.AppSettings["PostsToShow"], out int postsToShow) && postsToShow > 0)
                {
                    string userId = await _commonOperationsManager.VerifyToken(token);
                    var postsToReturn = await GetFollowedUsersPosts(userId, postsToShow);
                    if (postsToReturn.Count < postsToShow)
                    {
                        int postsAmountLeft = postsToShow - postsToReturn.Count;
                        IEnumerable<PostModel> publicPosts = await _postsRepository.GetPublicPosts(userId, postsAmountLeft);
                        foreach (var post in publicPosts)
                        {
                            postsToReturn.Add(post);
                        }
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
        /// Gets the posts that's been published by the users the specified user follow. 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        private async Task<ICollection<PostModel>> GetFollowedUsersPosts(string userId, int postsToShow)
        {
            try
            {
                IEnumerable<PostModel> postsToReturn = await _postsRepository.GetFollowedUsersPosts(userId, postsToShow);
                return postsToReturn.ToList();
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
