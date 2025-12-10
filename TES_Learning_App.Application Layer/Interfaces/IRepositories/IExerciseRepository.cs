using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IExerciseRepository : IGenericRepository<Exercise>
    {
        Task<IEnumerable<Exercise>> GetByActivityIdAsync(int activityId);
    }
}

