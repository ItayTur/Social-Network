using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUsersManager
    {
        /// <summary>
        /// Deletes the user associated with the specified token.
        /// </summary>
        /// <param name="token"></param>
        Task Delete(string token);
    }
}
