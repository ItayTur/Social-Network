using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AddUserToGraphException:Exception
    {
        public AddUserToGraphException()
        {
        }

        public AddUserToGraphException(string message) : base(message)
        {
        }

        public AddUserToGraphException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
