using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class PostWithTagsDto
    {
        public PostModel Post { get; set; }
        public IEnumerable<UserModel> Tags { get; set; }
        
    }
}
