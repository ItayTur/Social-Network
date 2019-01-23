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
        Task<XMPPAuthDto> Register(string token);
        Task DeleteUser(string token);
    }
}
