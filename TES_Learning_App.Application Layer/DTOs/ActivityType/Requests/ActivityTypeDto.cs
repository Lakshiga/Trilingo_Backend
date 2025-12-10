using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests
{
    public class ActivityTypeDto
    {
        public int Id { get; set; }
        public string Name_en { get; set; } = string.Empty;
        public string Name_ta { get; set; } = string.Empty;
        public string Name_si { get; set; } = string.Empty;
        public string? JsonMethod { get; set; }
        public int MainActivityId { get; set; }
    }
}
