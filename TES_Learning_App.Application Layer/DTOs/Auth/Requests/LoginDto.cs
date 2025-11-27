using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Auth.Requests
{
    public class LoginDto
    {
        [Required(ErrorMessage = "Username or Email is required")]
        [StringLength(255, ErrorMessage = "Identifier cannot exceed 255 characters")]
        public string Identifier { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; } = string.Empty;
    }
}
