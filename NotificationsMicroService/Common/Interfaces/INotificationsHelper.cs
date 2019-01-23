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
        Task Register(XMPPAuthDto authDto);
        //Task Login(string username, string password);
        Task SendMessageToUser(string message, string toUser);
    }
}
