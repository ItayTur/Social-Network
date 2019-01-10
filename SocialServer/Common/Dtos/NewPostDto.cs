using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class NewPostDto
    {
        public string Token { get; set; }

        public PostModel Post { get; set; }

        public ICollection<string> Tags { get; set; }
    }
}
