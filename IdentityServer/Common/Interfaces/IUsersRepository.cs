using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Interfaces
{
    public interface IUsersRepository
    {
        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        void Add(UserModel user);


        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        void Delete(string id);
    }
}
