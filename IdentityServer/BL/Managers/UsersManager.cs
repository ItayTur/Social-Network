using Common.Dtos;
using Common.Interfaces;
using Common.Models;
using System;
using System.Configuration;
using System.Net.Http;
using System.Security.Authentication;
using System.Threading.Tasks;
using System.Web;

namespace BL.Managers
{
    public class UsersManager : IUsersManager
    {

        private readonly IUsersRepository _usersRepository;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersRepository"></param>
        public UsersManager(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
        }



        /// <summary>
        /// Gets a user by Id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="token"></param>
        /// <returns>User</returns>
        public async Task<UserModel> Get(string token)
        {
            try
            {
                string userId = await VerfiyToken(token);
                return await _usersRepository.Get(userId);
            }
            catch (AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }



        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        public async Task Add(UserModel user, string token)
        {
            try
            {
                await VerfiyToken(token);
                await _usersRepository.AddOrUpdate(user);
            }
            catch (AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception e)
            {

                throw new Exception();
            }
        }


        
        /// <summary>
        /// Verifies the token validity.
        /// </summary>
        /// <param name="token"></param>
        private async Task<string> VerfiyToken(string token)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                try
                {
                    var authUrl = ConfigurationManager.AppSettings["AuthBaseUrl"] + "Auth";
                    var tokentDto = new TokenDto(token);
                    var response = await httpClient.PostAsJsonAsync(authUrl, tokentDto);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new AuthenticationException();
                    }
                    var content = await response.Content.ReadAsAsync<string>();
                    return content;
                }
                catch (Exception e)
                {

                    throw e;
                }

            }
        }



        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        public async Task Delete(string token)
        {
            try
            {
                string userId = await VerfiyToken(token);
                await _usersRepository.Delete(userId);

            }
            catch (Exception e)
            {

                throw e;
            }
        }



        /// <summary>
        /// Gets the full name of the user
        /// associated with id extracted from the token.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetFullName(string token)
        {
            try
            {
                string userId = await VerfiyToken(token);
                return await _usersRepository.GetFullName(userId);
            }
            catch (Exception)
            {

                throw;
            }
        }



        /// <summary>
        /// Gets the email of the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<string> GetUserEmailById(string token, string userId)
        {
            try
            {
                await VerfiyToken(token);
                UserModel user = await _usersRepository.Get(userId);
                return user.Email;
            }
            catch (Exception e)
            {

                throw e;
            }
        }



        /// <summary>
        /// Get the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserModel> GetUserById(string token, string userId)
        {
            try
            {
                await VerfiyToken(token);
                return await _usersRepository.Get(userId);
            }
            catch (AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception e)
            {
                throw new Exception();
            }
        }


        /// <summary>
        /// Updates the user associated with the Id extracted from the token.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        public async Task<UserModel> Update(string token, HttpRequest httpRequest)
        {
            try
            {
                string userId = await VerfiyToken(token);
                UserModel userToUpdate = GetUser(userId, httpRequest);
                await _usersRepository.AddOrUpdate(userToUpdate);
                return userToUpdate;

            }
            catch (AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception e)
            {

                throw new Exception();
            }
        }


        /// <summary>
        /// Gets user instance from thr http request specified.
        /// </summary>
        /// <param name="httpRequest"></param>
        /// <returns></returns>
        private UserModel GetUser(string userId, HttpRequest httpRequest)
        {
            return new UserModel
            {
                Id = userId,
                Address = httpRequest["Address"],
                BirthDate = DateTime.Parse(httpRequest["BirthDate"]),
                Email = httpRequest["Email"],
                FirstName = httpRequest["FirstName"],
                LastName = httpRequest["LastName"],
                Job = httpRequest["Job"]
            };
        }
    }
}
