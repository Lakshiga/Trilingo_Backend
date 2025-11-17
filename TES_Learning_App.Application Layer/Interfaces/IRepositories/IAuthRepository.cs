using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IAuthRepository
    {
        Task<User> RegisterAsync(User user, string password);
        Task<User?> GetUserByIdentifierAsync(string identifier);
        ///Task<User?> GetUserByEmailAsync(string email);
        Task<bool> UserExistsAsync(string email);

        Task<IEnumerable<User>> FindUsersByRoleIdAsync(Guid roleId);
    }
}
