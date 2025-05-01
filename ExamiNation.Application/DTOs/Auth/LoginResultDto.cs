using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ExamiNation.Application.DTOs.User;

namespace ExamiNation.Application.DTOs.Auth
{
    public class LoginResultDto
    {
        public string Token { get; set; }
        public UserLoginResponseDto User { get; set; }
    }

}
