using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IActivityTypeService
    {
        Task<IEnumerable<ActivityTypeDto>> GetAllAsync();
        Task<ActivityTypeDto?> GetByIdAsync(int id);
        Task<ActivityTypeDto> CreateAsync(CreateActivityTypeDto dto);
        Task UpdateAsync(int id, UpdateActivityTypeDto dto);
        Task DeleteAsync(int id);
    }
}
