using Common.Dtos;
using Common.Interfaces;
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

namespace NotificationsMicroService.Controllers
{
    public class NotificationsController : ApiController
    {
        private readonly INotificationsManager _notificationsManager;

        public NotificationsController(INotificationsManager notificationsManager)
        {
            _notificationsManager = notificationsManager;
        }

        /// <summary>
        /// Gets the user associated with the token specified.
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> PostRegister([FromBody] TokenDto token)
        {
            try
            {
                var authDto = await _notificationsManager.Register(token.Token);
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
    }
}
