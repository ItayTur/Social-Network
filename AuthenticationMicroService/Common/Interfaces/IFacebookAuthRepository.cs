using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IFacebookAuthRepository
    {               
        FacebookAuthModel GetAuthByFacebookId(string facebookId);

        Task Add(FacebookAuthModel newUser);

        bool IsFacebookIdFree(string facebookId);

        Task Delete(string facebookId);
    }
}

