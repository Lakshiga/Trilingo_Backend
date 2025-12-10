using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Response
{
    public class AuthResponseDto
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public string? Token { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string? Role { get; set; }
        public bool MustChangePassword { get; set; } = false;
    }
}
