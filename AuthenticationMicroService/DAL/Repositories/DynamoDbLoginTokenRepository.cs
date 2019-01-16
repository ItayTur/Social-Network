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
    public class DynamoDbLoginTokenRepository : ILoginTokenRepository
    {
        public async Task<LoginTokenModel> AddLoginToken(LoginTokenModel loginToken)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                await dbContext.SaveAsync(loginToken);
                var savedLoginToken= await dbContext.LoadAsync<LoginTokenModel>(loginToken.Token);
                return savedLoginToken;
            }
        }

        public LoginTokenModel GetLoginToken(string token)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {               
                var loginToken = dbContext.Load<LoginTokenModel>(token);
                return loginToken;
            }
        }

        public async Task Update(LoginTokenModel loginToken)
        {
            using (DynamoDbContext dbContext = new DynamoDbContext())
            {
                await dbContext.SaveAsync<LoginTokenModel>(loginToken);
            }
        }
    }
}
