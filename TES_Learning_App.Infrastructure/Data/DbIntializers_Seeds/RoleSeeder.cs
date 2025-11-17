using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds
{
    public static class RoleSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Check if any roles already exist.
            if (!context.Roles.Any())
            {
                var roles = new Role[]
                {
                    new Role { RoleName = "Parent" },
                    new Role { RoleName = "Admin" },
                    new Role { RoleName = "AdultLearner" }
                };

                context.Roles.AddRange(roles);
                context.SaveChanges();
            }
        }
    }
}
