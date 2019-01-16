using Common.Interfaces;
using Common.Models;
using DAL.Data;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Repositories
{
    public class DynamoDbAuthRepository : IAuthRepository
    {
        public async Task Add(AuthModel newUser)
        {
            try
            {
                using (DynamoDbContext dbContext = new DynamoDbContext())
                {
                    await dbContext.SaveAsync(newUser).ConfigureAwait(false);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
            
        }

        public bool IsEmailFree(string email)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                var auth = dbContext.Load<AuthModel>(email);
                return auth == null;
            }
        }

        public AuthModel GetAuthByEmail(string email)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                var user = dbContext.Load<AuthModel>(email);
                return user;
            }
        }

        public void Update(AuthModel updatedUser)
        {
            throw new NotImplementedException();
        }

        public async Task Delete(string userEmail)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                await dbContext.DeleteAsync<AuthModel>(userEmail);
                //TODO: log
            }
        }
    }
}
