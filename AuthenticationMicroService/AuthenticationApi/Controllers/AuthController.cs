using Common.Dtos;
using Common.Interfaces;
using System;
using System.Data.Linq;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AuthenticationApi.Controllers
{
    public class AuthController : ApiController
    {
        private IAuthManager _authManager;

        public AuthController(IAuthManager authManager)
        {
            _authManager = authManager;
        }

        [HttpPost]
        [Route("api/Auth/FacebookSignIn")]
        public IHttpActionResult FacebookSignIn([FromBody]AccessTokenDto accessToken)
        {
            try
            {
                var appToken =  _authManager.FacebookSignIn(accessToken.AccessToken);
                return Ok(appToken);
            }
            catch (ArgumentException e)
            {

                return BadRequest(e.Message);
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

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
