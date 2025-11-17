using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds
{
    public static class LanguageSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Check if any languages already exist.
            if (!context.Languages.Any())
            {
                var languages = new Language[]
                {
                    // --- THESE LINES ARE NOW CORRECTED ---
                    // We are setting the LanguageName property, which exists.
                    new Language { LanguageName = "English" },
                    new Language { LanguageName = "Tamil" },
                    new Language { LanguageName = "Sinhala" }
                };

                context.Languages.AddRange(languages);
                context.SaveChanges();
            }
        }

    }
}

