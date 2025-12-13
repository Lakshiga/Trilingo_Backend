using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Role.Requests;
using TES_Learning_App.Application_Layer.DTOs.Role.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IRoleService
    {
        Task<IEnumerable<RoleDto>> GetAllAsync();
        Task<RoleDto?> GetByIdAsync(Guid id);
        Task<RoleDto> CreateAsync(CreateRoleDto dto);
        Task UpdateAsync(Guid id, UpdateRoleDto dto);
        Task DeleteAsync(Guid id);
    }
}
