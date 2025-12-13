using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // Use a dummy connection string for design-time migrations
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=Trilingo_DesignTime;Trusted_Connection=true;MultipleActiveResultSets=true");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}