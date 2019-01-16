using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IAuthRepository
    {        
        AuthModel GetAuthByEmail(string email);

        void Update(AuthModel updatedUser);

        void Add(AuthModel newUser);

        bool IsEmailFree(string email);

        Task Delete(string userEmail);
    }
}
