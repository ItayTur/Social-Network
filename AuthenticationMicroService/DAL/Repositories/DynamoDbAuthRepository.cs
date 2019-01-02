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
        public string AddFacebookLoginAndLogin(string facebookToken)
        {
            throw new NotImplementedException();
        }

        public string AddUsernamePasswordLoginAndLogin(string email, string password)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                AssertEmailIsFree(dbContext, email);
                var newUser = new AuthModel() { Email = email, Password = password };
                dbContext.Save(newUser);
                return LoginByUsernamePassword(email, password);
            }
        }

        public void ChangePassword(string accessToken, string oldPassword, string newPassword)
        {
            throw new NotImplementedException();
        }

        public void DeactivateUser(string email)
        {
            throw new NotImplementedException();
        }

        public string LoginByFacebookToken(string facebookToken)
        {
            throw new NotImplementedException();
        }

        public string LoginByUsernamePassword(string email, string password)
        {
            throw new NotImplementedException();
        }

        public bool ValidateAccessToken(string accessToken)
        {
            throw new NotImplementedException();
        }

        private void AssertEmailIsFree(DynamoDbContext dbContext, string email)
        {
            var duplicateUser = dbContext.Load<AuthModel>(email);
            if (duplicateUser != null)
            {
                throw new DuplicateKeyException(duplicateUser);
            }
        }
    }
}
