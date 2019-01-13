using Common;
using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web;
using System.Web.Http;

namespace SocialServer.Controllers
{
    public class PostsController : ApiController
    {
        private readonly IPostsManager _postsManager;

        public PostsController(IPostsManager postsManager)
        {
            _postsManager = postsManager;
        }

        [HttpPost]
        [Route("api/Posts/AddPost")]
        public IHttpActionResult AddPost()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                CookieHeaderValue cookie = Request.Headers.GetCookies("authToken").FirstOrDefault();
                NewPostDto newPost = new NewPostDto
                {
                    Content = httpRequest["Content"],
                    Date = DateTime.Now,
                    //Token = cookie["token"].Value,
                };
                
                var picFile = httpRequest.Files["pic"];
                _postsManager.Add(newPost);
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
