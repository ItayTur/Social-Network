using Common.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class NewPostDto
    {
        public string Token { get; set; }

        public PostModel Post { get; }

        public string Content { get; set; }

        public DateTime Date { get; set; }

        //public File Pic { get; set; }

        public ICollection<string> Tags { get; set; }

        public NewPostDto()
        {
            Post = new PostModel() { Content = Content, DateTime = Date };
        }
    }
}
