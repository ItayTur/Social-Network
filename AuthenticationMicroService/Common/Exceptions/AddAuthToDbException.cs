using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Exceptions
{
    public class AddAuthToDbException: Exception
    {
        public AddAuthToDbException()
        {
        }

        public AddAuthToDbException(string message) : base(message)
        {
        }

        public AddAuthToDbException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
