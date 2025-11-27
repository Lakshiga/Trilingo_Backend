using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Activity.Requests
{
    public class UpdateActivityDto
    {
        public string? Details_JSON { get; set; }

        [StringLength(500, ErrorMessage = "Name_en cannot exceed 500 characters")]
        public string? Name_en { get; set; }

        [StringLength(500, ErrorMessage = "Name_ta cannot exceed 500 characters")]
        public string? Name_ta { get; set; }

        [StringLength(500, ErrorMessage = "Name_si cannot exceed 500 characters")]
        public string? Name_si { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "SequenceOrder must be greater than 0")]
        public int? SequenceOrder { get; set; }
    }
}
