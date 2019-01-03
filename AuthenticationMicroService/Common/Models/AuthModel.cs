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
        public bool IsActive { get; set; }
        public bool IsDeleted { get; set; }

        public AuthModel()
        {

        }

        public AuthModel(string email, string password)
        {
            Email = email;
            Password = password;
            IsActive = true;
        }
    }

}
