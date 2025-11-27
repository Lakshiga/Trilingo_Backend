using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class LoginDto
    {
        //public string Email { get; set; } = string.Empty;

        // This field can now accept either a Username or an Email.
        public string Identifier { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;


    }
}
