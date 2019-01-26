using Common.Interfaces;
using Common.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using FromBodyAttribute = System.Web.Http.FromBodyAttribute;

namespace IdentityServer.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersManager _usersManager;

        /// <summary>
        /// COnstructor.
        /// </summary>
        /// <param name="usersManager"></param>
        public UsersController(IUsersManager usersManager)
        {
            _usersManager = usersManager;
        }


        /// <summary>
        /// Gets the user associated with the token specified.
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetUser()
        {
            try
            {
                string token = GetCookie(Request, ConfigurationManager.AppSettings["UserTokenCookieName"]);
                var user  = await _usersManager.Get(token);
                return Ok(user);
            }
            catch (AuthenticationException)
            {
                return BadRequest("Authentication was not approved");
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        /// <summary>
        /// Adds user to the DB.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> PostUser([FromBody] JObject data)
        {
            try
            {
                UserModel user = data["user"].ToObject<UserModel>();
                string token = data["token"].ToObject<string>();
                await _usersManager.Add(user, token);
                return Ok();
            }
            catch (AuthenticationException)
            {
                return BadRequest("Authentication was not approved");
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        /// <summary>
        /// Retrieves an individual cookie from the cookies collection
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        private string GetCookie(HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            throw new AuthenticationException();
        }


        /// <summary>
        /// Deletes the user assoiciated with the id extracted from the token specified. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Web.Http.HttpDelete]
        [System.Web.Http.Route("api/Users/DeleteUser/{token}")]
        public async Task<IHttpActionResult> DeleteUser(string token)
        {
            try
            {
                await _usersManager.Delete(token);
                return Ok();
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


       
        /// <summary>
        /// Gets the full name of the user
        /// assoiciated with the id extracted from the token specified. 
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        [System.Web.Http.Route("api/Users/GetFullName/{token}")]
        public async Task<IHttpActionResult> GetFullName(string token)
        {
            try
            {
                string fullName = await _usersManager.GetFullName(token);
                return Ok(fullName);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }

        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Users/GetUserById")]
        public async Task<IHttpActionResult>GetUserById(JObject data)
        {
            try
            {
                string token = data["token"].ToObject<string>();
                string userId = data["userId"].ToObject<string>();
                //UserModel user = await _usersManager.GetUserById(token ,userId);

                return Ok(user);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        [System.Web.Http.HttpPost]
        [System.Web.Http.Route("api/Users/GetUserEmailById")]
        public async Task<IHttpActionResult> GetUserEmailById(JObject data)
        {
            try
            {
                string token = data["token"].ToObject<string>();
                string userId = data["userId"].ToObject<string>();
                string emailToReturn = await _usersManager.GetUserEmailById(token, userId);
                return Ok(emailToReturn);
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }
    }
}
