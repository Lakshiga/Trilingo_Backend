using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Student.Response
{
    public class StudentDto
    {
        public Guid Id { get; set; }
        public string Nickname { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public int Age { get; set; } // The calculated age!
        public int XpPoints { get; set; } // The gamification score!
        public string NativeLanguageCode { get; set; } = string.Empty;
        public string TargetLanguageCode { get; set; } = string.Empty;
    }

}
