using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace AuthenticationApi.Controllers
{
    public class AuthController : ApiController
    {
        IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        public IHttpActionResult RegisterUsernamePassword(string email, string password)
        {
            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                return BadRequest("One of the parameters was missing");
            }

            try
            {
                var token = _authManager.RegisterUserByUsernamePasswordAndLogin(email, password);
                return Ok(token);
            }
            catch (DuplicateKeyException ex)
            {
                return BadRequest("Email already in use");
            }
            catch (Exception ex)
            {
                return BadRequest("Internal server error");
            }
        }

    }
}
