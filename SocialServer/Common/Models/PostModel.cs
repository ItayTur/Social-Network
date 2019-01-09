using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Models
{
    public class PostModel
    {
        public string Content { get; set; }
        public string ImgUrl { get; set; }
        public DateTime DateTime { get; set; }
        public int Likes { get; set; }
    }
}
