using Common.Interfaces;
using Common.Models;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DynamoDbUsersRepository : IUsersRepository
    {
        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        public void Add(UserModel user)
        {
            using(DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    context.Save(user);
                }
                catch (Exception e)
                {
                    //ADD LOGER
                    throw new Exception(e.Message);
                }
                
            }
        }

        /// <summary>
        /// Deletes the user associated with the specified id.
        /// </summary>
        /// <param name="id"></param>
        public void Delete(string id)
        {
            using (DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    context.Delete<UserModel>(id);
                }
                catch (Exception e)
                {
                    //ADD LOGER
                    throw new Exception(e.Message);
                }

            }
        }
        
    }
}
