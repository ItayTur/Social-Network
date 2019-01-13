using Common;
using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
        public IHttpActionResult AddPost(NewPostDto newPost)
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var picFile = httpRequest.Files["pic"];
                _postsManager.Add(newPost);
                return Ok();

            }
            catch (Exception)
            {
                return InternalServerError();
            }
        }
    }
}
