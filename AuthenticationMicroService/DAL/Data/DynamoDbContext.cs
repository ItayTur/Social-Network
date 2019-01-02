using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class DynamoDbContext: DynamoDBContext
    {
        private static AmazonDynamoDBClient client = new AmazonDynamoDBClient();

        public DynamoDbContext() : base(client, new DynamoDBContextConfig { ConsistentRead = true, SkipVersionCheck = true })
        {
        }
    }
}
