using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Admin.Requests;
using TES_Learning_App.Application_Layer.DTOs.Admin.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IAdminService
    {
        Task<AdminDto> CreateAdminAsync(CreateAdminDto dto);
        Task<AdminDto?> GetAdminByIdAsync(Guid adminId);
        Task<IEnumerable<AdminListDto>> GetAllAdminsAsync();
        Task<AdminDto> UpdateAdminAsync(Guid adminId, UpdateAdminDto dto);
        Task DeleteAdminAsync(Guid adminId);
        Task<AdminDto> ResetAdminPasswordAsync(Guid adminId, ResetAdminPasswordDto dto);
    }
}
