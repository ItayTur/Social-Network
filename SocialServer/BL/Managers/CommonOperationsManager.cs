using Common;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Authentication;
using System.Text;
using System.Threading.Tasks;

namespace BL.Managers
{
    public class CommonOperationsManager : ICommonOperationsManager
    {
        private readonly string _authBaseUrl;


        public CommonOperationsManager()
        {
            _authBaseUrl = ConfigurationManager.AppSettings["AuthBaseUrl"];
        }


        /// <summary>
        /// Gets the value of the cookie name specified.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cookieName"></param>
        /// <returns></returns>
        public string GetCookieValue(HttpRequestMessage request, string cookieName)
        {
            CookieHeaderValue cookie = request.Headers.GetCookies(cookieName).FirstOrDefault();
            if (cookie != null)
                return cookie[cookieName].Value;

            throw new AuthenticationException();
        }

        /// <summary>
        /// Gets a new guid instance as string.
        /// </summary>
        /// <returns></returns>
        public string GetNewGuid()
        {
            return Guid.NewGuid().ToString();
        }


        /// <summary>
        /// Verifies the specified token and return the user id.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<string> VerifyToken(string token)
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
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }
    }
}
