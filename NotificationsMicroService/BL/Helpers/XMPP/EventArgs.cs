using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Helpers.XMPP
{
    public class EventArgs<T> : EventArgs
    {
        public T Data { get; set; }

        public EventArgs(T data)
        {
            Data = data;
        }
    }
}
