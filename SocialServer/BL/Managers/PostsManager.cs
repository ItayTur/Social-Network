using Amazon;
using Amazon.S3;
using Common;
using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace BL.Managers
{
    public class PostsManager : IPostsManager
    {

        private readonly IPostsRepository _postsRepository;
        private readonly IStorageManager _storageManager;
        private readonly string _authBaseUrl;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="postsRepository"></param>
        /// <param name="storageManager"></param>
        public PostsManager(IPostsRepository postsRepository, IStorageManager storageManager)
        {
            _postsRepository = postsRepository;
            _storageManager = storageManager;
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
                var userId = await VerifyToken(token).ConfigureAwait(false);
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
                string taggerId = await VerifyToken(token);
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
        /// Verifies the specified token and return the user id.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> VerifyToken(string token)
        {
            TokenDto tokenDto = new TokenDto() { Token = token };
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync(_authBaseUrl, tokenDto).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new AuthenticationException();
                    }
                    else
                    {
                        return await response.Content.ReadAsAsync<string>();
                    }
                }
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
            
        }
    }
}
