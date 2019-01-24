using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface INotificationsManager
    {
        /// <summary>
        /// Registers a new user to the notifications server.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<NotificationsAuthDto> Register(string token);

        /// <summary>
        /// Delets a registered user.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task DeleteUser(string token);

        /// <summary>
        /// Gets the notifications auth.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        Task<NotificationsAuthDto> GetNotificationsAuth(string token);

        /// <summary>
        /// Sends a message to a user.
        /// </summary>
        /// <param name="notificationMessageDto"></param>
        /// <returns></returns>
        Task SendMessageToUser(NotificationMessageDto notificationMessageDto);
    }
}
