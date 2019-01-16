using Common.Interfaces;
using Common.Models;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;

namespace SocialServer.Controllers
{
    public class UsersController : ApiController
    {
        private readonly IUsersManager _usersManager;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersManager"></param>
        public UsersController(IUsersManager usersManager)
        {
            _usersManager = usersManager;
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
                VerifyUserData(token, email);
                await _usersManager.Add(token, email);
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
        private void VerifyUserData(string token, string email)
        {
            if (token == null || email == null || token == "" || email == "")
            {
                throw new ArgumentNullException();
            }
        }


        /// <summary>
        /// Deletes the user associated with the specified ID.
        /// </summary>
        /// <returns></returns>
        [HttpDelete]
        [Route("api/users/DeleteUserByToken/{token}")]
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
    }
}
