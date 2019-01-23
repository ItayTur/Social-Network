using Common.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class UserWithRelationsDto
    {
        public UserModel User { get; set; }

        public bool IsFollow { get; set; }

        public bool IsBlock { get; set; }
    }
}
