using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.MainActivity.Response
{
    public class MainActivityDto
    {
        public int Id { get; set; }
        public string Name_en { get; set; } = string.Empty;
        public string Name_ta { get; set; } = string.Empty;
        public string Name_si { get; set; } = string.Empty;
    }
}
