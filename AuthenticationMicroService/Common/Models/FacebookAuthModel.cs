using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    [DynamoDBTable("FacebookAuth")]
    public class FacebookAuthModel  
    {
        [DynamoDBHashKey]
        public string FacebookId { get; set; }
        public string UserId { get; set; }
        public bool IsBLocked { get; set; }
        public bool IsDeleted { get; set; }


        public FacebookAuthModel()
        {

        }

        public FacebookAuthModel(string facebookId, string userId)
        {
            FacebookId = facebookId;
            UserId = userId;
        }
    }
}
