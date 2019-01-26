using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class UserBlockedException: Exception
    {
        public UserBlockedException()
        {
        }

        public UserBlockedException(string message) : base(message)
        {
        }

        public UserBlockedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
