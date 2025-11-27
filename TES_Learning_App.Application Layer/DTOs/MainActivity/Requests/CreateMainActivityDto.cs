using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests
{
    public class CreateMainActivityDto
    {
        [Required(ErrorMessage = "English name is required")]
        [StringLength(200, ErrorMessage = "Name_en cannot exceed 200 characters")]
        public string Name_en { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name_ta cannot exceed 200 characters")]
        public string Name_ta { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name_si cannot exceed 200 characters")]
        public string Name_si { get; set; } = string.Empty;
    }
}
