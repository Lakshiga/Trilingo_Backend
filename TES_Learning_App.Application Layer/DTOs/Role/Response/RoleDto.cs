using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Role.Response
{
    public class RoleDto
    {
        public Guid Id { get; set; } // Remember, Role uses Guid
        public string RoleName { get; set; } = string.Empty;
    }
}
