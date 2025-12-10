using TES_Learning_App.Application_Layer.DTOs.Progress.Requests;
using TES_Learning_App.Application_Layer.DTOs.Progress.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IStudentProgressService
    {
        Task<ProgressDto> CreateProgressAsync(CreateProgressDto dto, Guid parentUserId);
        Task<ProgressDto?> GetProgressByIdAsync(int id, Guid? parentUserId = null);
        Task<PagedProgressResponse> GetFilteredProgressAsync(ProgressFilterDto filter, Guid? parentUserId = null);
        Task<IEnumerable<ProgressDto>> GetStudentProgressAsync(Guid studentId, Guid? parentUserId = null);
        Task<ProgressSummaryDto> GetStudentSummaryAsync(Guid studentId, Guid? parentUserId = null);
        Task<StudentStatsDto> GetStudentStatsAsync(Guid studentId, Guid? parentUserId = null);
        Task<ProgressDto> UpdateProgressAsync(int id, UpdateProgressDto dto, Guid? parentUserId = null);
        Task<bool> DeleteProgressAsync(int id, Guid? parentUserId = null);
        Task<bool> ValidateStudentOwnershipAsync(Guid studentId, Guid parentUserId);
    }
}


