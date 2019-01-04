using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
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
        public string UserEmail { get; set; }
        public DateTime CreationTime { get; set; }
        public DateTime ExpiredTime { get; set; }

        public LoginTokenModel()
        {

        }

        public LoginTokenModel(string email)
        {
            UserEmail = email;
            CreationTime = DateTime.Now;
            ExpiredTime = CreationTime.AddMinutes(15);
            Token = Guid.NewGuid().ToString();
        }
    }
}
