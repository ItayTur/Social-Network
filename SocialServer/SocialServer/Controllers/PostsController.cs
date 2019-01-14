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
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> AddPost()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                CookieHeaderValue cookie = Request.Headers.GetCookies("authToken").FirstOrDefault();

                string picPath = "";
                if (httpRequest.Files["Pic"] != null)
                {
                    picPath = HttpContext.Current.Server.MapPath("~/" + httpRequest.Files["pic"].FileName);
                }
                
                await _postsManager.Add(httpRequest,"cookieToken", picPath);
                return Ok();

            }
            catch (Exception e)
            {
                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("api/Posts/SearchTag/{text}")]
        public async Task<IHttpActionResult> SearchTag(string text)
        {
            try
            {
                IEnumerable<UserModel> tagFound = await _postsManager.SearchTag(text);
                return Ok(tagFound);
            }
            catch (ArgumentException e)
            {

                return NotFound();
            }
            catch(Exception e)
            {
                return InternalServerError();
            }
        }
    }
}
