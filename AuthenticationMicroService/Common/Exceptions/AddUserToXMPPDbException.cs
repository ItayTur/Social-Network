using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AddUserToXMPPDbException : Exception
    {
        public AddUserToXMPPDbException()
        {
        }

        public AddUserToXMPPDbException(string message) : base(message)
        {
        }

        public AddUserToXMPPDbException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
