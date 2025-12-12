using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Repositories
{
    public class AuthRepository : IAuthRepository
    {
        private readonly ApplicationDbContext _context;
        public AuthRepository(ApplicationDbContext context) { _context = context; }

        public async Task<User?> GetUserByIdentifierAsync(string identifier)
        {
            // The Story: The clerk receives an "identifier". It needs to figure out what it is.
            // It first converts the identifier to lowercase for a consistent, case-insensitive check.
            var normalizedIdentifier = identifier.ToLower();

            // It then goes to the Users cabinet and looks for a file where EITHER
            // the Username matches OR the Email matches.
            // It also makes sure to load the related Role information at the same time.
            return await _context.Users
                .Include(u => u.Role)
                .SingleOrDefaultAsync(u => u.Username.ToLower() == normalizedIdentifier || u.Email.ToLower() == normalizedIdentifier);
        }

        public async Task<User> RegisterAsync(User user, string password)
        {
            CreatePasswordHash(password, out byte[] passwordHash, out byte[] passwordSalt);
            user.PasswordHash = passwordHash;
            user.PasswordSalt = passwordSalt;
            var parentRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Parent");
            if (parentRole == null)
            {
                parentRole = new Role
                {
                    Id = Guid.NewGuid(),
                    RoleName = "Parent"
                };
                await _context.Roles.AddAsync(parentRole);
                // Save immediately so RoleId is available and unique constraint (if any) is satisfied
                await _context.SaveChangesAsync();
            }
            user.RoleId = parentRole.Id;
            user.Role = parentRole;
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task<bool> UserExistsAsync(string email)
        {
            return await _context.Users.AnyAsync(u => u.Email.ToLower() == email.ToLower());
        }

        public async Task<bool> UserExistsByUsernameAsync(string username)
        {
            return await _context.Users.AnyAsync(u => u.Username.ToLower() == username.ToLower());
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
            }
        }

        public async Task<IEnumerable<User>> FindUsersByRoleIdAsync(Guid roleId)
        {
            return await _context.Users
                                 .Where(u => u.RoleId == roleId)
                                 .ToListAsync();
        }

        public async Task<User?> GetUserByResetTokenAsync(string token)
        {
            return await _context.Users
                .Include(u => u.Role)
                .FirstOrDefaultAsync(u => u.PasswordResetToken == token);
        }
    }
}
