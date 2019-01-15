using Common.Dtos;
using Common.Interfaces;
using System;
using System.Data.Linq;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Cors;

namespace AuthenticationApi.Controllers
{
    public class AuthController : ApiController
    {
        private IAuthManager _authManager;
        private ILoginTokenManager _loginTokenManager;
        private IFacebookAuthManager _facebookAuthManager;

        public AuthController(IAuthManager authManager, ILoginTokenManager loginTokenManager, IFacebookAuthManager facebookAuthManager)
        {
            _authManager = authManager;
            _loginTokenManager = loginTokenManager;
            _facebookAuthManager = facebookAuthManager;
        }

        [HttpPost]
        [Route("api/Auth/FacebookSignIn")]
        public async Task<IHttpActionResult> FacebookSignIn([FromBody]AccessTokenDto accessToken)
        {
            try
            {
                var appToken =  await _facebookAuthManager.SignIn(accessToken.Token);
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
        [Route("api/Auth/Register")]
        public async Task<IHttpActionResult> RegisterUsernamePasswordAsync([FromBody] RegistrationDto registrationDto)
        {
            if (DtoNotValid(registrationDto))
            {
                return BadRequest("One of the parameters was missing");
            }

            try
            {
                string token = await _authManager.RegisterUserAndLogin(registrationDto);
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
                var token = _authManager.LoginUser(authDto.Email, authDto.Password);
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
            if (authDto == null 
                || string.IsNullOrWhiteSpace(authDto.Email) 
                || string.IsNullOrWhiteSpace(authDto.Password))
            {
                return true;
            }
            return false;
        }

        private bool DtoNotValid(RegistrationDto registrationInfoDto)
        {
            if (registrationInfoDto == null 
                || string.IsNullOrWhiteSpace(registrationInfoDto.Email) 
                || string.IsNullOrWhiteSpace(registrationInfoDto.Password) 
                || string.IsNullOrWhiteSpace(registrationInfoDto.FirstName) 
                || string.IsNullOrWhiteSpace(registrationInfoDto.LastName))
            {
                return true;
            }
            return false;
        }

        [HttpPost]
        public async Task<IHttpActionResult> VerifyAuth([FromBody]AccessTokenDto accessToken)
        {
            try
            {
                var userId = await _loginTokenManager.VerifyAsync(accessToken.Token);
                return Ok(userId);
            }
            catch (AuthenticationException )
            {

                return BadRequest("Authentication was not approved");
            }
            catch (Exception e)
            {
                return InternalServerError();
            }

        }

    }
}
