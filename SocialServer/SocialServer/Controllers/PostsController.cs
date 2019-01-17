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
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SocialServer.Controllers
{
    public class PostsController : ApiController
    {
        private readonly IPostsManager _postsManager;
        private readonly ICommonOperationsManager _commonOperationsManager;

        public PostsController(IPostsManager postsManager, ICommonOperationsManager commonOperationsManager)
        {
            _postsManager = postsManager;
            _commonOperationsManager = commonOperationsManager;
        }

        [HttpPost]
        [Route("api/Posts/AddPost")]
        public async Task<IHttpActionResult> AddPost()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                string picPath = "";
                if (httpRequest.Files["Pic"] != null)
                {
                    picPath = HttpContext.Current.Server.MapPath("~/" + httpRequest.Files["pic"].FileName);
                }
                await _postsManager.Add(httpRequest, token, picPath);
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
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                IEnumerable<UserModel> tagsFound = await _postsManager.SearchTag(text, token);
                return Ok(tagsFound);
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

       
        [HttpGet]
        [Route("api/Posts/GetUsersPosts")]
        public async Task<IHttpActionResult> GetUsersPosts()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var posts = await _postsManager.GetPosts(token);
                return Ok(posts);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }
    }
}
