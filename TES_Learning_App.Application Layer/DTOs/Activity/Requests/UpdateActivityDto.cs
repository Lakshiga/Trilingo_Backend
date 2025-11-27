using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Activity.Requests
{
    public class UpdateActivityDto
    {
        public string? Details_JSON { get; set; }
        public string? Name_en { get; set; }
        public string? Name_ta { get; set; }
        public string? Name_si { get; set; }
        public int? SequenceOrder { get; set; }
        // We typically wouldn't allow changing the core relationships, just the content.
    }
}
