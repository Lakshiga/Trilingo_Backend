using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TES_Learning_App.Domain.Common
{
    public abstract class BaseTranslation
    {
        // These are the shared, reusable properties for translations
        public string Name_en { get; set; } = string.Empty;
        public string Name_ta { get; set; } = string.Empty;
        public string Name_si { get; set; } = string.Empty;


        //this class has no Id.It is not an entity itself, just a template.
    }
}
