using Common.Interfaces;
using Common.Models;
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

namespace IdentityServer.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersManager _usersManager;


        public UsersController(IUsersManager usersManager)
        {
            _usersManager = usersManager;
        }

        public async Task<IHttpActionResult> GetUser(string id)
        {
            try
            {
                string token = GetCookie(Request, ConfigurationManager.AppSettings["UserTokenCookieName"]);
                await _usersManager.Get(id, token);
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

        public async Task<IHttpActionResult> PostUser(UserModel user, string token)
        {
            try
            {
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
