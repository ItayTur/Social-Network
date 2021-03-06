﻿using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
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
                    picPath = HttpContext.Current.Server.MapPath("~/" + httpRequest.Files["Pic"].FileName);
                }
                var postToReturn = await _postsManager.Add(httpRequest, token, picPath);
                return Ok(postToReturn);

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
            catch (Exception e)
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


        [HttpPost]
        [Route("api/Posts/AddComment")]
        public async Task<IHttpActionResult> AddComment()
        {
            try
            {
                var httpRequest = HttpContext.Current.Request;
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                string picPath = "";
                if (httpRequest.Files["Pic"] != null)
                {
                    picPath = HttpContext.Current.Server.MapPath("~/"+ httpRequest.Files["Pic"].FileName);
                }
                var commentAdded = await _postsManager.AddComment(httpRequest, token, picPath);
                return Ok(commentAdded);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

        [HttpGet]
        [Route("api/Posts/GetCommentsOfPost/{postId}")]
        public async Task<IHttpActionResult> GetCommentsOfPost(string postId)
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var comments = await _postsManager.GetCommentsOfPost(postId, token);
                return Ok(comments);
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
