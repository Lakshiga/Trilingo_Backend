using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests
{
    public class UpdateMainActivityDto
    {
        [StringLength(200, ErrorMessage = "Name_en cannot exceed 200 characters")]
        [JsonPropertyName("name_en")]
        public string? Name_en { get; set; }
        
        [StringLength(200, ErrorMessage = "Name_ta cannot exceed 200 characters")]
        [JsonPropertyName("name_ta")]
        public string? Name_ta { get; set; }
        
        [StringLength(200, ErrorMessage = "Name_si cannot exceed 200 characters")]
        [JsonPropertyName("name_si")]
        public string? Name_si { get; set; }
    }
}
