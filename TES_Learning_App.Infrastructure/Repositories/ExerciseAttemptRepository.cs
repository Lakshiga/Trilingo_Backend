using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Repositories
{
    /// <summary>
    /// Repository implementation for ExerciseAttempt entity
    /// </summary>
    public class ExerciseAttemptRepository : GenericRepository<ExerciseAttempt>, IExerciseAttemptRepository
    {
        public ExerciseAttemptRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}