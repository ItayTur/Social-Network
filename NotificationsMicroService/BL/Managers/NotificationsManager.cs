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
        private readonly ICommonOperationsManager _commonOperationsManager;

        public NotificationsManager(INotificationsHelper notificationsHelper, ICommonOperationsManager commonOperationsManager)
        {
            _notificationsHelper = notificationsHelper;
            _commonOperationsManager = commonOperationsManager;
        }
       
        /// <summary>
        /// Registers a new user to the notifications server.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<NotificationsAuthDto> Register(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                var auth = new NotificationsAuthDto() { Username = userId, Password = userId };
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
        /// Delets a registered user.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task DeleteUser(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                await _notificationsHelper.DeleteUser(userId);
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
        /// Gets the notifications auth.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public async Task<NotificationsAuthDto> GetNotificationsAuth(string token)
        {
            try
            {
                string userId = await _commonOperationsManager.VerifyToken(token);
                var auth = new NotificationsAuthDto() { Username = userId, Password = userId };
                return auth;
            }
            catch (AuthenticationException)
            {
                throw new AuthenticationException();
            }
            catch (Exception e)
            {

                throw new Exception(e.Message);
            }
        }

        /// <summary>
        /// Sends a message to a user.
        /// </summary>
        /// <param name="notificationMessageDto"></param>
        /// <returns></returns>
        public async Task SendMessageToUser(NotificationMessageDto notificationMessageDto)
        {
            try
            {
                VerifyNotificationData(notificationMessageDto);
                string userId = await _commonOperationsManager.VerifyToken(notificationMessageDto.AppToken);
                var auth = new NotificationsAuthDto() { Username = userId, Password = userId };
                await _notificationsHelper.SendMessageToUser(notificationMessageDto.Message, notificationMessageDto.UserId);
            }
            catch (ArgumentNullException)
            {
                throw new ArgumentNullException();
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
        /// Verifies the token and email specified.
        /// </summary>
        private void VerifyNotificationData(NotificationMessageDto notificationMessageDto)
        {
            if (notificationMessageDto == null 
                || string.IsNullOrWhiteSpace(notificationMessageDto.AppToken) 
                || string.IsNullOrWhiteSpace(notificationMessageDto.Message) 
                || string.IsNullOrWhiteSpace(notificationMessageDto.UserId))
            {
                throw new ArgumentNullException();
            }
        }
    }
}
