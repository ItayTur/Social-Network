using Common.Dtos;
using Common.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers.XMPP
{
    public class XMPPNotificationsHelper : INotificationsHelper
    {
        private readonly XMPP _xmpp;

        public XMPPNotificationsHelper()
        {
            _xmpp = new XMPP();
        }


        /// <summary>
        /// Registers a user to the notifications service.
        /// </summary>
        /// <param name="authDto"></param>
        /// <returns></returns>
        public async Task Register(NotificationsAuthDto authDto) => await _xmpp.Register(authDto.Username, authDto.Password);

        /// <summary>
        /// Sends a message to a specific user.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="toUser"></param>
        /// <returns></returns>
        public async Task SendMessageToUser(string message, string toUser) => await _xmpp.SendPrivateMessage(message, toUser);
        
        /// <summary>
        /// Delets a registered user.
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        public async Task DeleteUser(string userName) => await _xmpp.RemoveUser(userName);
    }
}
