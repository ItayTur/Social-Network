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

        Task Update(AuthModel updatedUser);

        Task Add(AuthModel newUser);

        bool IsEmailFree(string email);

        Task Delete(string userEmail);
    }
}
