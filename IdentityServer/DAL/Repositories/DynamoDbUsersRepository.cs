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
        /// Gets a user record from the db.
        /// </summary>
        /// <param name="id"></param>
        /// <returns>User</returns>
        public async Task<UserModel> Get(string id)
        {
            using (DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    return await context.LoadAsync<UserModel>(id);

                }
                catch (Exception e)
                {
                    //ADD LOGER
                    throw new Exception(e.Message);
                }
            }
        }



        /// <summary>
        /// Adds new user record to the db.
        /// </summary>
        /// <param name="user"></param>
        public async Task Add(UserModel user)
        {
            using(DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    await context.SaveAsync(user);
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
        public async Task Delete(string id)
        {
            using (DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    await context.DeleteAsync<UserModel>(id);
                }
                catch (Exception e)
                {
                    //ADD LOGER
                    throw new Exception(e.Message);
                }

            }
        }



        /// <summary>
        /// Gets the full name of the user associated with id specified.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<string> GetFullName(string id)
        {
            using (DynamoDbContext context = new DynamoDbContext())
            {
                try
                {
                    var userSearched = await context.LoadAsync<UserModel>(id);
                    return userSearched.FirstName + " " + userSearched.LastName;

                }
                catch (Exception e)
                {
                    //ADD LOGER
                    throw new Exception(e.Message);
                }
            }
        }



        /// <summary>
        /// Gets the email of the user associated with the specified Id.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<UserModel> GetUserById(string userId)
        {
            try
            {
                using (DynamoDbContext context = new DynamoDbContext())
                {
                    return await context.LoadAsync<UserModel>(userId);
                } 
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
