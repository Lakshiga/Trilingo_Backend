using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IRepositories
{
    public interface IStudentProgressRepository : IGenericRepository<StudentProgress>
    {
        Task<StudentProgress?> GetByIdAsync(int id);
        Task<IEnumerable<StudentProgress>> GetByStudentIdAsync(Guid studentId);
        Task<IEnumerable<StudentProgress>> GetByActivityIdAsync(int activityId);
        Task<StudentProgress?> GetLatestByStudentAndActivityAsync(Guid studentId, int activityId);
        Task<IEnumerable<StudentProgress>> GetFilteredAsync(
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
            bool sortDescending = true);
        Task<int> GetCountAsync(
            Guid? studentId = null,
            int? activityId = null,
            int? stageId = null,
            int? levelId = null,
            DateTime? startDate = null,
            DateTime? endDate = null,
            int? minScore = null,
            int? maxScore = null,
            bool? isCompleted = null);
    }
}


