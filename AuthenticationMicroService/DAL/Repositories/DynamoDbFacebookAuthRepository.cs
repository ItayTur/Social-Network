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
        public void Add(FacebookAuthModel newUser)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                dbContext.Save(newUser);
            }
        }

        public void Delete(string facebookId)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                dbContext.Delete<FacebookAuthModel>(facebookId);
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
    }
}
