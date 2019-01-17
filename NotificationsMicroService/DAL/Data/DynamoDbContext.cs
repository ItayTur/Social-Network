using Amazon.DynamoDBv2;
using Amazon.DynamoDBv2.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Data
{
    public class DynamoDbContext : DynamoDBContext
    {
        private static AmazonDynamoDBClient dynamoDBClient = new AmazonDynamoDBClient();

        public DynamoDbContext() : base(dynamoDBClient, new DynamoDBContextConfig { ConsistentRead = true, SkipVersionCheck = true })
        {
        }
    }
}
