using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class PasswordException: Exception
    {
        public PasswordException()
        {
        }

        public PasswordException(string message) : base(message)
        {
        }

        public PasswordException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
