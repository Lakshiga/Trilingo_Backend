using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Activity.Requests;
using TES_Learning_App.Application_Layer.DTOs.Activity.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IActivityService
    {
        Task<IEnumerable<ActivityDto>> GetAllAsync();
        Task<IEnumerable<ActivityDto>> GetByStageIdAsync(int stageId);
        Task<ActivityDto?> GetByIdAsync(int id);
        Task<ActivityDto> CreateAsync(CreateActivityDto dto);
        Task UpdateAsync(int id, UpdateActivityDto dto);
        Task DeleteAsync(int id);
    }
}