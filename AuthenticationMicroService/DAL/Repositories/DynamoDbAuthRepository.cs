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
        public AuthModel Add(AuthModel newUser)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                dbContext.Save(newUser);
                var savedNewUser = dbContext.Load<AuthModel>(newUser.Email);
                return savedNewUser;
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

        public void Delete(string userEmail)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                dbContext.Delete<AuthModel>(userEmail);
                //TODO: log
            }
        }
    }
}
