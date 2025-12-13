using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests
{
    public class UpdateMainActivityDto
    {
        // Frontend sends PascalCase (Name_en, Name_ta, Name_si)
        // JsonPropertyName allows accepting camelCase for backward compatibility
        [JsonPropertyName("name_en")]
        public string? Name_en { get; set; }
        
        [JsonPropertyName("name_ta")]
        public string? Name_ta { get; set; }
        
        [JsonPropertyName("name_si")]
        public string? Name_si { get; set; }
    }
}
