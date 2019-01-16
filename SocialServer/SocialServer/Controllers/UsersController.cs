using Common.Interfaces;
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
