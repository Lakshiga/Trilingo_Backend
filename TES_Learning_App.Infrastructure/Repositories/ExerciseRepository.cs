using Microsoft.EntityFrameworkCore;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Repositories
{
    public class ExerciseRepository : GenericRepository<Exercise>, IExerciseRepository
    {
        public ExerciseRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<IEnumerable<Exercise>> GetByActivityIdAsync(int activityId)
        {
            return await _context.Set<Exercise>()
                .Where(e => e.ActivityId == activityId)
                .OrderBy(e => e.SequenceOrder)
                .ToListAsync();
        }
    }
}

