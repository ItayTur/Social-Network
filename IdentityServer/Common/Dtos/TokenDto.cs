using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Dtos
{
    public class TokenDto
    {
        public string Token { get; }

        public TokenDto(string token)
        {
            Token = token;
        }

        public TokenDto()
        {

        }
    }
}
