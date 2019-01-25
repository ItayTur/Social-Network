using Common.Interfaces;
using Newtonsoft.Json.Linq;
using System;
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


        [HttpGet]
        [Route("api/Users/GetFollowers")]
        /// <summary>
        /// Gets the followers of the user associated with Id extracted from the token.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name=""></param>
        /// <returns></returns>
        public async Task<IHttpActionResult> GetFollowers()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                await _usersManager.GetFollowers(token);
                return Ok();
            }
            catch (Exception e)
            {

                return InternalServerError();
            }
        }


        /// <summary>
        /// Creats follow relation.
        /// </summary>
        /// <returns></returns>
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


        /// <summary>
        /// Deletes follow relation.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users/DeleteFollow")]
        public async Task<IHttpActionResult> DeleteFollow()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var httpRequest = HttpContext.Current.Request;
                await _usersManager.DeleteFollow(token, httpRequest);
                return Ok();
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        /// <summary>
        /// Creates block connection.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users/CreateBlock")]
        public async Task<IHttpActionResult> CreateBlock()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var httpRequest = HttpContext.Current.Request;
                await _usersManager.CreateBlock(token, httpRequest);
                return Ok();
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }


        /// <summary>
        /// Deletes block relation.
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("api/Users/DeleteBlock")]
        public async Task<IHttpActionResult> DeleteBlock()
        {
            try
            {
                string token = _commonOperationsManager.GetCookieValue(Request, "authToken");
                var httpRequest = HttpContext.Current.Request;
                await _usersManager.DeleteBlock(token, httpRequest);
                return Ok();
            }
            catch (Exception)
            {

                return InternalServerError();
            }
        }
    }
}
