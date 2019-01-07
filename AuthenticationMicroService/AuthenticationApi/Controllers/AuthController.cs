using Common.Dtos;
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
                return false;
            }
            return true;
        }
    }
}
