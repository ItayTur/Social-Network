using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class UsersManager : IUsersManager
    {

        private readonly IUsersRepository _usersRepository;
        private readonly string _authBaseUrl;


        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="usersRepository"></param>
        public UsersManager(IUsersRepository usersRepository)
        {
            _usersRepository = usersRepository;
            _authBaseUrl = ConfigurationManager.AppSettings["AuthBaseUrl"];
        }

        public async Task Delete( string token)
        {
            try
            {
                string userId = await VerifyToken(token);
                await _usersRepository.Delete(userId);
            }
            catch (AuthenticationException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                throw e;
            }
        }

        /// <summary>
        /// Verifies the specified token and return the user id.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        private async Task<string> VerifyToken(string token)
        {
            TokenDto tokenDto = new TokenDto() { Token = token };
            try
            {
                using (HttpClient httpClient = new HttpClient())
                {
                    var response = await httpClient.PostAsJsonAsync(_authBaseUrl, tokenDto).ConfigureAwait(false);
                    if (!response.IsSuccessStatusCode)
                    {
                        throw new AuthenticationException();
                    }
                    else
                    {
                        return await response.Content.ReadAsAsync<string>();
                    }
                }
            }
            catch (AuthenticationException e)
            {
                throw e;
            }
            catch (Exception e)
            {

                throw e;
            }

        }
    }
}
