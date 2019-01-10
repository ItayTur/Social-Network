using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DynamoDBTable("LoginTokens")]
    public class LoginTokenModel
    {
        [DynamoDBHashKey]
        public string Token { get; set; }
        public string UserId { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public string LoginType { get; set; }

        public LoginTokenModel()
        {

        }

        public LoginTokenModel(string userId, LoginTypes loginType)
        {
            UserId = userId;
            LoginType = loginType.ToString();
            CreationTime = DateTime.Now;
            int AccessTokenMinutes = int.Parse(ConfigurationManager.AppSettings["AccessTokenMinutes"]);
            ExpiredTime = CreationTime.AddMinutes(AccessTokenMinutes);
            Token = Guid.NewGuid().ToString();
        }

        
        public enum LoginTypes
        {
            UserPassword,
            Facebook
        }
    }
}
