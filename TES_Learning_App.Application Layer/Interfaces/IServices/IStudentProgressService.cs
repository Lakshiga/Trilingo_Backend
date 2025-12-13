using System;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Student.Request;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IStudentProgressService
    {
        Task<ExerciseResultResponseDto> SubmitExerciseResultAsync(Guid studentId, SubmitExerciseResultDto dto);
        // மாணவர் செயல்திறன் சுருக்கத்தைப் பெறுவதற்கான புதிய முறை
        Task<ProgressSummaryDto> GetStudentProgressSummaryAsync(Guid studentId);
    }
}