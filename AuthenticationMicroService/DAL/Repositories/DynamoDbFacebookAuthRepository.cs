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
    public class DynamoDbFacebookAuthRepository : IFacebookAuthRepository
    {
        public async Task Add(FacebookAuthModel newUser)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                await dbContext.SaveAsync(newUser);
            }
        }

        public async Task Delete(string facebookId)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                await dbContext.DeleteAsync<FacebookAuthModel>(facebookId).ConfigureAwait(false);
                //TODO: log
            }
        }

        public FacebookAuthModel GetAuthByFacebookId(string facebookId)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                var user = dbContext.Load<FacebookAuthModel>(facebookId);
                return user;
            }
        }

        public bool IsFacebookIdFree(string facebookId)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                var auth = dbContext.Load<FacebookAuthModel>(facebookId);
                return auth == null;
            }
        }

        public async Task Update(FacebookAuthModel facebookAuth)
        {
            try
            {
                using (DynamoDbContext dbContext = new DynamoDbContext())
                {
                    await dbContext.SaveAsync(facebookAuth);
                }
            }
            catch (Exception e)
            {

                throw e;
            }
        }
    }
}
