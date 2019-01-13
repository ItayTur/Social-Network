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
                string path = HttpContext.Current.Server.MapPath("~/" + httpRequest.Files["pic"].FileName);
                
                _postsManager.Add(httpRequest,"cookieToken", path);
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
