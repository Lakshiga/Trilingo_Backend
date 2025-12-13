using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Application_Layer.DTOs.Student.Requests
{
    public class UpdateStudentDto
    {
        public string Nickname { get; set; } = string.Empty;
        public string Avatar { get; set; } = string.Empty;
        public string NativeLanguageCode { get; set; } = string.Empty;
        public string TargetLanguageCode { get; set; } = string.Empty;
    }


}
