using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Level.Requests;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface ILevelService
    {
        Task<IEnumerable<LevelDto>> GetAllAsync();
        Task<LevelDto?> GetByIdAsync(int id);
        Task<LevelDto> CreateAsync(CreateLevelDto dto);
        Task UpdateAsync(int id, UpdateLevelDto dto);
        Task DeleteAsync(int id);
    }
}
