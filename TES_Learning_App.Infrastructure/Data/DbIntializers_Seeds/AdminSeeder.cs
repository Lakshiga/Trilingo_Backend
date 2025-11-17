using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds
{
    public static class AdminSeeder
    {
        public static void Seed(ApplicationDbContext context)
        {
            // Check if admin user already exists
            var existingAdminUser = context.Users
                .Include(u => u.Role)
                .FirstOrDefault(u => u.Username == "admin" || u.Email == "admin@teslearning.com");
            
            if (existingAdminUser != null)
            {
                Console.WriteLine("Admin user already exists, skipping creation.");
                return;
            }

            Console.WriteLine("Creating admin user...");
            
            // Get the Admin role
            var adminRole = context.Roles.FirstOrDefault(r => r.RoleName == "Admin");
            if (adminRole == null)
            {
                throw new InvalidOperationException("Admin role not found. Make sure RoleSeeder runs before AdminSeeder.");
            }

            // Create password hash for default admin
            CreatePasswordHash("Admin123!", out byte[] passwordHash, out byte[] passwordSalt);

            // Create default admin user
            var adminUser = new User
            {
                Id = Guid.NewGuid(),
                Username = "admin",
                Email = "admin@teslearning.com",
                PasswordHash = passwordHash,
                PasswordSalt = passwordSalt,
                RoleId = adminRole.Id,
                Role = adminRole
            };

            context.Users.Add(adminUser);
            context.SaveChanges(); // Save user first

            // Create admin profile
            var adminProfile = new Admin
            {
                Id = Guid.NewGuid(),
                UserId = adminUser.Id,
                User = adminUser,
                FullName = "System Administrator",
                JobTitle = "System Administrator"
            };

            context.Admins.Add(adminProfile);
            context.SaveChanges();
            Console.WriteLine("Admin user created successfully!");
        }

        private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
