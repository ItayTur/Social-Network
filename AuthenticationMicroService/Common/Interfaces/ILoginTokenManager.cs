using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Common.Models.LoginTokenModel;

namespace Common.Interfaces
{
    public interface ILoginTokenManager
    {
        string Add(string userId, LoginTypes loginType);
    }
}
