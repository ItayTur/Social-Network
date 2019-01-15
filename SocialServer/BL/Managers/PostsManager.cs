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

                PostModel post = new PostModel()
                {
                    Content = httpRequest["Content"],
                    DateTime = DateTime.Now
                };
                List<TagDto> tags = new List<TagDto>();
                if (httpRequest["Tags"] != null)
                {
                    tags = JsonConvert.DeserializeObject<List<TagDto>>((httpRequest["Tags"]));
                }
                HttpPostedFile picFile = httpRequest.Files["Pic"];
                if (picFile != null)
                {
                    post.ImgUrl = await _storageManager.AddPicToStorage(picFile, path).ConfigureAwait(false);
                }
                //var userId = await VerifyToken("692dc1cd-ec5d-46e5-83ed-12e0bb6fa87d").ConfigureAwait(false);
                var userId = "8776290e-793a-430a-8cfe-e5e800bc50ab";
                post.Id = GenerateId();
                var addPostToDbTask = _postsRepository.Add(userId, post, tags);
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }


        /// <summary>
        /// Searches the text specified and return the matching results.
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public async Task<IEnumerable<UserModel>> SearchTag(string text)
        {
            return await _postsRepository.GetUsersOfEmailWith(text);
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
        /// Verifies the specified token.
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
