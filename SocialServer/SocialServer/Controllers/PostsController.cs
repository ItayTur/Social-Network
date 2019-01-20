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
                    string fileName = _commonOperationsManager.GetNewGuid();
                    picPath = HttpContext.Current.Server.MapPath("~/" + fileName);
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


        [HttpPost]
        [Route("api/Posts/LikePost")]
        public async Task<IHttpActionResult> LikePost()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                await _postsManager.LikePost(token, httpRequest);
                return Ok();

            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        [HttpPost]
        [Route("api/Posts/IsPostLikedBy")]
        public async Task<IHttpActionResult> IsPostLikedBy()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                var token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var isPostLiked = await _postsManager.IsPostLikedBy(token, httpRequest);
                return Ok(isPostLiked);

            }
            catch (Exception e)
            {

                return InternalServerError();
            }
        }


        [HttpPost]
        [Route("api/Posts/UnLikePost")]
        public async Task<IHttpActionResult> UnLikePost()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                await _postsManager.UnLikePost(token, httpRequest);
                return Ok();

            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

    }
}
