using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.ActivityType.Response
{
    public class CreateActivityTypeDto
    {
        [Required(ErrorMessage = "English name is required")]
        [StringLength(200, ErrorMessage = "Name_en cannot exceed 200 characters")]
        public string Name_en { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name_ta cannot exceed 200 characters")]
        public string Name_ta { get; set; } = string.Empty;

        [StringLength(200, ErrorMessage = "Name_si cannot exceed 200 characters")]
        public string Name_si { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "JsonMethod cannot exceed 500 characters")]
        public string? JsonMethod { get; set; }

        [Required(ErrorMessage = "MainActivityId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "MainActivityId must be greater than 0")]
        public int MainActivityId { get; set; }
    }
}
