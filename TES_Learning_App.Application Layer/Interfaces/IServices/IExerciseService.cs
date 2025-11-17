using System.Collections.Generic;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Requests;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IExerciseService
    {
        Task<IEnumerable<ExerciseDto>> GetAllAsync();
        Task<IEnumerable<ExerciseDto>> GetByActivityIdAsync(int activityId);
        Task<ExerciseDto?> GetByIdAsync(int id);
        Task<ExerciseDto> CreateAsync(CreateExerciseDto dto);
        Task UpdateAsync(int id, UpdateExerciseDto dto);
        Task DeleteAsync(int id);
    }
}
