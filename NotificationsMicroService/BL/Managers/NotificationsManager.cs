using Common.Dtos;
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
    public class NotificationsManager : INotificationsManager
    {
        private readonly INotificationsHelper _notificationsHelper;

        public NotificationsManager(INotificationsHelper notificationsHelper)
        {
            _notificationsHelper = notificationsHelper;
        }

        public async Task<XMPPAuthDto> Register(string token)
        {
            try
            {
               // string userId = await VerfiyToken(token);
                var auth = new XMPPAuthDto() { Username = token, Password = token };
                await _notificationsHelper.Register(auth);
                return auth;
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
    }
}
