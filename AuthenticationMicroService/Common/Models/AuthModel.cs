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
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
