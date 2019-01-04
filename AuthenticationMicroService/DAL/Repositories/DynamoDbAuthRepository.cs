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
        /*
        public string AddFacebookLoginAndLogin(string facebookToken)
        {
            throw new NotImplementedException();
        }

        public AuthModel AddAuthByUsernamePassword(AuthModel newUser)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                //AssertEmailIsFree(dbContext, email);
                //var newUser = new AuthModel() { Email = email, Password = password };
                dbContext.Save(newUser);
                return dbContext.Load(newUser);
                //return LoginByUsernamePassword(email, password);
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

        public LoginTokenModel GetAccessToken(string accessToken)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                return dbContext.Load<LoginTokenModel>(accessToken);
            }
        }

        private void AssertEmailIsFree(DynamoDbContext dbContext, string email)
        {
            var duplicateUser = dbContext.Load<AuthModel>(email);
            if (duplicateUser != null)
            {
                throw new DuplicateKeyException(duplicateUser);
            }
        }*/
        public AuthModel Add(AuthModel newUser)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                dbContext.Save(newUser);
                var savedNewUser = dbContext.Load<AuthModel>(newUser.Email);
                return savedNewUser;
            }
        }

        public bool CheckIfEmailIsFree(string email)
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
    }
}
