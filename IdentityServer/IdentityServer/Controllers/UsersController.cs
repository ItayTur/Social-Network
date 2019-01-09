using Common.Interfaces;
using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
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
    }
}
