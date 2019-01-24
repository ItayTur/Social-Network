using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;

namespace SocialServer.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersManager _usersManager;
        private readonly ICommonOperationsManager _commonOperationsManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersManager"></param>
        public UsersController(IUsersManager usersManager, ICommonOperationsManager commonOperationsManager)
        {
            _usersManager = usersManager;
            _commonOperationsManager = commonOperationsManager;
        }


        /// <summary>
        /// Adds user node to the graph db.
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> AddUser([FromBody] JObject data)
        {
            try
            {
                string token = data["token"].ToObject<string>();
                string email = data["email"].ToObject<string>();
                string name = data["name"].ToObject<string>();
                VerifyUserData(token, email, name);
                await _usersManager.Add(token, email, name);
                return Ok();
            }
            catch (Exception)
            {

                throw;
            }
            
        }



        /// <summary>
        /// Verifies the token and email specified.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="email"></param>
        private void VerifyUserData(string token, string email, string name)
        {
            if (string.IsNullOrWhiteSpace(token) || string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentNullException();
            }
        }


        [HttpDelete]
        [Route("api/users/DeleteUserByToken/{token}")]
        /// <summary>
        /// Deletes the user associated with the specified ID.
        /// </summary>
        /// <returns></returns>
        public async Task<IHttpActionResult> DeleteUserByToken(string token)
        {
            try
            {
                await _usersManager.Delete(token);
                return Ok();
            }
            catch (Exception e)
            {

                return InternalServerError();
            }
        }



        [HttpGet]
        [Route("api/users/GetUsers")]
        /// <summary>
        /// Gets all the users except the user associated with the specified Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="usersToShow"></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetUsers()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var usersToReturn = await _usersManager.GetUsers(token);
                return Ok(usersToReturn);
            }
            catch (Exception e)
            {

                return InternalServerError();
            }
        } 


        [HttpPost]
        [Route("api/Users/CreateFollow")]
        public async Task<IHttpActionResult> CreateFollow()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var httpRequest = HttpContext.Current.Request;
                await _usersManager.CreateFollow(token, httpRequest);
                return Ok();
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


    }
}
