using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Activity.Requests
{
    public class CreateActivityDto
    {
        public string? Details_JSON { get; set; }

        [Required(ErrorMessage = "StageId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "StageId must be greater than 0")]
        public int StageId { get; set; }

        [Required(ErrorMessage = "MainActivityId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "MainActivityId must be greater than 0")]
        public int MainActivityId { get; set; }

        [Required(ErrorMessage = "ActivityTypeId is required")]
        [Range(1, int.MaxValue, ErrorMessage = "ActivityTypeId must be greater than 0")]
        public int ActivityTypeId { get; set; }

        [Required(ErrorMessage = "English name is required")]
        [StringLength(500, ErrorMessage = "Name_en cannot exceed 500 characters")]
        public string Name_en { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Name_ta cannot exceed 500 characters")]
        public string Name_ta { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Name_si cannot exceed 500 characters")]
        public string Name_si { get; set; } = string.Empty;

        [Range(1, int.MaxValue, ErrorMessage = "SequenceOrder must be greater than 0")]
        public int SequenceOrder { get; set; } = 1;
    }
}
