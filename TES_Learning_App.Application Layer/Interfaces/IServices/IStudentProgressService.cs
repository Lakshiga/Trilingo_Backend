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
        
        /// <summary>
        /// Submits an exercise attempt and handles scoring logic
        /// </summary>
        /// <param name="studentId">ID of the student</param>
        /// <param name="dto">Exercise attempt details</param>
        /// <returns>Result of the exercise submission</returns>
        Task<ExerciseResultResponseDto> SubmitExerciseAttemptAsync(Guid studentId, SubmitExerciseAttemptDto dto);
    }
}