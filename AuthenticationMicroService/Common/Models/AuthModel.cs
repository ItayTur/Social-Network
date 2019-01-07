using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DynamoDBTable("Auth")]
    public class AuthModel
    {
        [DynamoDBHashKey]
        public string Email { get; set; }
        public string Password { get; set; }
        public bool IsBLocked { get; set; }
        public bool IsDeleted { get; set; }

        public AuthModel()
        {

        }

        public AuthModel(string email, string password = null)
        {
            Email = email;
            Password = password;
        }
    }

}
