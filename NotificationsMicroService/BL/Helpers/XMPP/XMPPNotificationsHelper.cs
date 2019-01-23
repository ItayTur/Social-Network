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
        

        // public Task Login(string username, string password) => _xmpp.Connect(username, password);

        public async Task Register(XMPPAuthDto authDto) => await _xmpp.Register(authDto.Username, authDto.Password);

        public async Task SendMessageToUser(string message, string toUser) => await _xmpp.SendPrivateMessage(message, toUser);

        public async Task DeleteUser(string userName) => await _xmpp.RemoveUser(userName);
    }
}
