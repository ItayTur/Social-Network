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


        public UsersController(IUsersManager usersManager)
        {
            _usersManager = usersManager;
        }

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

        public async Task<IHttpActionResult> PostUser([FromBody] JObject data)
        {
            try
            {
                UserModel user = data["user"].ToObject<UserModel>();
                string token = data["token"].ToObject<String>();
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
        public static string GetCookie(HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            throw new AuthenticationException();
        }
    }
}
