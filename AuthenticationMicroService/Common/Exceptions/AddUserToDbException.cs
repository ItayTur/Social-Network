using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AddUserToDbException: Exception
    {
        public AddUserToDbException()
        {
        }

        public AddUserToDbException(string message): base(message)
        {
        }

        public AddUserToDbException(string message, Exception inner): base(message, inner)
        {
        }
    }
}
