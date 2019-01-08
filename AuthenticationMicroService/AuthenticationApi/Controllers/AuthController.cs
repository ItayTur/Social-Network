using Common.Dtos;
using Common.Interfaces;
using System;
using System.Data.Linq;
using System.Threading.Tasks;
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
        public async Task<IHttpActionResult> FacebookSignIn([FromBody]AccessTokenDto accessToken)
        {
            try
            {
                var appToken =  await _authManager.FacebookSignIn(accessToken.AccessToken);
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
        public IHttpActionResult RegisterUsernamePassword([FromBody] AuthDto authDto)
        {
            if (DtoNotValid(authDto))
            {
                return BadRequest("One of the parameters was missing");
            }

            try
            {
                var token = _authManager.RegisterUserByUsernamePasswordAndLogin(authDto.Email, authDto.Password);
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

        [HttpPost]
        [Route("api/Auth/Login")]
        public IHttpActionResult LoginUsernamePassword([FromBody] AuthDto authDto)
        {
            if (DtoNotValid(authDto))
            {
                return BadRequest("One of the parameters was missing");
            }

            try
            {
                var token = _authManager.LoginUserByUserPassword(authDto.Email, authDto.Password);
                return Ok(token);
            }
            catch (ArgumentException ex)
            {
                return BadRequest("Incorrect email address and / or password");
            }
            catch (Exception ex)
            {
                return BadRequest("Internal server error");
            }
        }

        private bool DtoNotValid(AuthDto authDto)
        {
            if (authDto == null || string.IsNullOrWhiteSpace(authDto.Email) || string.IsNullOrWhiteSpace(authDto.Password))
            {
                return true;
            }
            return false;
        }
    }
}
