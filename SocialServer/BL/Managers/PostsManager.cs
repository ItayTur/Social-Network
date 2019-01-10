using Common;
using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class PostsManager : IPostsManager
    {

        private readonly IPostsRepository _postsRepository;

        public PostsManager(IPostsRepository postsRepository)
        {
            _postsRepository = postsRepository;
        }

        /// <summary>
        /// Adds a post to the database. 
        /// </summary>
        /// <param name="posterId"></param>
        /// <param name="post"></param>
        /// <param name="tagIds"></param>
        public async Task Add(NewPostDto newPostDto)
        {
            try
            {
                await VerifyToken(newPostDto.Token);
                string postId = GenerateId();
                await _postsRepository.Add(postId, newPostDto.Post, newPostDto.Tags);
            }
            catch (Exception)
            {

                throw new Exception();
            }
        }

        private string GenerateId()
        {
            return Guid.NewGuid().ToString();
        }

        private async Task VerifyToken(string token)
        {
            TokenDto tokenDto = new TokenDto() { Token = token };
            using(HttpClient httpClient = new HttpClient())
            {
                var response = await httpClient.PutAsJsonAsync("localHost:53535", tokenDto);
                if (!response.IsSuccessStatusCode)
                {
                    throw new AuthenticationException();
                }
            }
        }
    }
}
