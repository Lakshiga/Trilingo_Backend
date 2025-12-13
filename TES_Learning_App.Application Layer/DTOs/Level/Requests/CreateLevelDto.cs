using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Level.Requests
{
    public class CreateLevelDto
    {
        public string Name_en { get; set; }
        public string Name_ta { get; set; }
        public string Name_si { get; set; }
        public int LanguageId { get; set; }
    }
}
