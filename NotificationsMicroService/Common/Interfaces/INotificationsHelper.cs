using Common.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface INotificationsHelper
    {
        /// <summary>
        /// Registers a user to the notifications service.
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        Task Register(NotificationsAuthDto authDto);

        /// <summary>
        /// Sends a message to a specific user.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUser"></param>
        /// <returns></returns>
        Task SendMessageToUser(string message, string toUser);

        /// <summary>
        /// Delets a registered user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task DeleteUser(string userName);
    }
}
