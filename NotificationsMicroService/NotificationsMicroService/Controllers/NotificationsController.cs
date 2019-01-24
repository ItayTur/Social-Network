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
        private readonly ICommonOperationsManager _commonOperationsManager;

        public NotificationsController(INotificationsManager notificationsManager, ICommonOperationsManager commonOperationsManager)
        {
            _notificationsManager = notificationsManager;
            _commonOperationsManager = commonOperationsManager;
        }

        /// <summary>
        /// Adds a user for notifications associated with the token.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Notifications/Register")]
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
        /// Gets notification authentication.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns>User</returns>
        public async Task<IHttpActionResult> GetUserAuth()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, ConfigurationManager.AppSettings["UserTokenCookieName"]);
                var notificationsAuth = await _notificationsManager.GetNotificationsAuth(token);
                return Ok(notificationsAuth);
            }
            catch (AuthenticationException)
            {
                return BadRequest("Authentication was not approved");
            }
            catch (Exception e)
            {
                return InternalServerError(e);
            }
        }


        /// <summary>
        /// Send a message to a user.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Notifications/SendMessage")]
        public async Task<IHttpActionResult> PostSendMessage([FromBody] NotificationMessageDto notificationMessageDto)
        {
            try
            {
                await _notificationsManager.SendMessageToUser(notificationMessageDto);
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
    }
}
