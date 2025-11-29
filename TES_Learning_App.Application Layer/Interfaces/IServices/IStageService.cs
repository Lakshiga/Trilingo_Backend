using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Stage.Requests;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IStageService
    {
        Task<IEnumerable<StageDto>> GetAllAsync();
        Task<StageDto?> GetByIdAsync(int id);
        Task<IEnumerable<StageDto>> GetByLevelIdAsync(int levelId); // Add this method
        Task<StageDto> CreateAsync(CreateStageDto dto);
        Task UpdateAsync(int id, UpdateStageDto dto);
        Task DeleteAsync(int id);
    }
}