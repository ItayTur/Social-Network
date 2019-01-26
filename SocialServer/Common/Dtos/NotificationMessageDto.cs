using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class NotificationMessageDto
    {
        public string UserId { get; set; }       
        public string Message { get; set; }       
        public string AppToken { get; set; }       
    }
}
