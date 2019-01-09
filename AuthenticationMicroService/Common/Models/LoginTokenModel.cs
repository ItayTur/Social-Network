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
        public string Id { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpiredTime { get; set; }
        public LoginTypes LoginType { get; set; }

        public LoginTokenModel()
        {

        }

        public LoginTokenModel(string id, LoginTypes loginType)
        {
            Id = id;
            LoginType = loginType;
            CreationTime = DateTime.Now;
            int AccessTokenMinutes = int.Parse(ConfigurationManager.AppSettings["AccessTokenMinutes"]);
            ExpiredTime = CreationTime.AddMinutes(AccessTokenMinutes);
            Token = Guid.NewGuid().ToString();
        }

        [Flags]
        public enum LoginTypes
        {
            UserPassword = 0,
            Facebook = 1
        }
    }
}
