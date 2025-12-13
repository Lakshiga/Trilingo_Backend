using Microsoft.EntityFrameworkCore;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Infrastructure.Data;

namespace TES_Learning_App.Infrastructure.Repositories
{
    public class StudentProgressRepository : GenericRepository<StudentProgress>, IStudentProgressRepository
    {
        public StudentProgressRepository(ApplicationDbContext context) : base(context) { }

        public async Task<IEnumerable<StudentProgress>> GetByStudentIdAsync(Guid studentId)
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Activity)
                    .ThenInclude(a => a.Stage)
                        .ThenInclude(s => s.Level)
                .Where(sp => sp.StudentId == studentId)
                .OrderByDescending(sp => sp.CompletedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<StudentProgress>> GetByActivityIdAsync(int activityId)
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Student)
                .Where(sp => sp.ActivityId == activityId)
                .OrderByDescending(sp => sp.CompletedAt)
                .ToListAsync();
        }

        public async Task<StudentProgress?> GetLatestByStudentAndActivityAsync(Guid studentId, int activityId)
        {
            return await _context.StudentProgresses
                .Include(sp => sp.Activity)
                .Where(sp => sp.StudentId == studentId && sp.ActivityId == activityId)
                .OrderByDescending(sp => sp.CompletedAt)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<StudentProgress>> GetFilteredAsync(
            Guid? studentId = null,
            int? activityId = null,
            int? stageId = null,
            int? levelId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? minScore = null,
            int? maxScore = null,
            bool? isCompleted = null,
            int skip = 0,
            int take = 20,
            string? sortBy = "CompletedAt",
            bool sortDescending = true)
        {
            var query = _context.StudentProgresses
                .Include(sp => sp.Student)
                .Include(sp => sp.Activity)
                    .ThenInclude(a => a.Stage)
                        .ThenInclude(s => s.Level)
                .AsQueryable();

            // Apply filters
            if (studentId.HasValue)
                query = query.Where(sp => sp.StudentId == studentId);

            if (activityId.HasValue)
                query = query.Where(sp => sp.ActivityId == activityId);

            if (stageId.HasValue)
                query = query.Where(sp => sp.Activity.StageId == stageId);

            if (levelId.HasValue)
                query = query.Where(sp => sp.Activity.Stage.LevelId == levelId);

            if (startDate.HasValue)
                query = query.Where(sp => sp.CompletedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(sp => sp.CompletedAt <= endDate.Value);

            if (minScore.HasValue)
                query = query.Where(sp => sp.Score >= minScore.Value);

            if (maxScore.HasValue)
                query = query.Where(sp => sp.Score <= maxScore.Value);

            if (isCompleted.HasValue)
                query = query.Where(sp => sp.IsCompleted == isCompleted.Value);

            // Apply sorting
            query = sortBy?.ToLower() switch
            {
                "score" => sortDescending
                    ? query.OrderByDescending(sp => sp.Score)
                    : query.OrderBy(sp => sp.Score),
                "timespentseconds" => sortDescending
                    ? query.OrderByDescending(sp => sp.TimeSpentSeconds)
                    : query.OrderBy(sp => sp.TimeSpentSeconds),
                "completedat" or _ => sortDescending
                    ? query.OrderByDescending(sp => sp.CompletedAt)
                    : query.OrderBy(sp => sp.CompletedAt)
            };

            return await query
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> GetCountAsync(
            Guid? studentId = null,
            int? activityId = null,
            int? stageId = null,
            int? levelId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? minScore = null,
            int? maxScore = null,
            bool? isCompleted = null)
        {
            var query = _context.StudentProgresses.AsQueryable();

            if (studentId.HasValue)
                query = query.Where(sp => sp.StudentId == studentId);

            if (activityId.HasValue)
                query = query.Where(sp => sp.ActivityId == activityId);

            if (stageId.HasValue)
                query = query.Where(sp => sp.Activity.StageId == stageId);

            if (levelId.HasValue)
                query = query.Where(sp => sp.Activity.Stage.LevelId == levelId);

            if (startDate.HasValue)
                query = query.Where(sp => sp.CompletedAt >= startDate.Value);

            if (endDate.HasValue)
                query = query.Where(sp => sp.CompletedAt <= endDate.Value);

            if (minScore.HasValue)
                query = query.Where(sp => sp.Score >= minScore.Value);

            if (maxScore.HasValue)
                query = query.Where(sp => sp.Score <= maxScore.Value);

            if (isCompleted.HasValue)
                query = query.Where(sp => sp.IsCompleted == isCompleted.Value);

            return await query.CountAsync();
        }
    }
}


