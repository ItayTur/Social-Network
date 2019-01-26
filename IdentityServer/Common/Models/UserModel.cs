using Amazon.DynamoDBv2.DataModel;
using Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DynamoDBTable("Users")]
    public class UserModel
    {
        [DynamoDBHashKey]
        public string Id { get; set; }
        public string Email { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Address { get; set; }
        public string Job { get; set; }
        public string RegistrationType { get; set; }
        public string RegistrationKey { get; set; }
        public DateTime BirthDate { get; set; }
    }
}
