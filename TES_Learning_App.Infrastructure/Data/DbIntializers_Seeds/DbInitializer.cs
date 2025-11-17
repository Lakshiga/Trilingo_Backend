using Microsoft.EntityFrameworkCore;
using System.Linq;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Apply any pending migrations
            if (context.Database.GetPendingMigrations().Any())
            {
                context.Database.Migrate();
            }

            // Call the seed methods from other seed classes
            LanguageSeeder.Seed(context);
            RoleSeeder.Seed(context);
            AdminSeeder.Seed(context);

            // Add calls to other seeders as needed
            // Example:
            // SomeOtherSeeder.Seed(context);
        }
    }
}
