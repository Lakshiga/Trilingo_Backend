using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Progress.Requests;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [ApiController]
    [Route("api/admin/[controller]")]
    [Authorize(Roles = "Admin,SuperAdmin")]
    public class AdminProgressController : BaseApiController
    {
        private readonly IStudentProgressService _progressService;

        public AdminProgressController(IStudentProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Get all progress records with filters (Admin only)
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProgress([FromQuery] ProgressFilterDto filter)
        {
            var result = await _progressService.GetFilteredProgressAsync(filter);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific progress record by ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetProgressById(int id)
        {
            var result = await _progressService.GetProgressByIdAsync(id);
            return HandleGetById(result, "Progress", id);
        }

        /// <summary>
        /// Get progress for a specific student
        /// </summary>
        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> GetStudentProgress(Guid studentId)
        {
            var result = await _progressService.GetStudentProgressAsync(studentId);
            return Ok(result);
        }

        /// <summary>
        /// Get statistics for all students
        /// </summary>
        [HttpGet("stats")]
        public async Task<IActionResult> GetStatistics([FromQuery] Guid? studentId)
        {
            if (studentId.HasValue)
            {
                var stats = await _progressService.GetStudentStatsAsync(studentId.Value);
                return Ok(stats);
            }

            // Return overall statistics (can be enhanced later)
            return Ok(new { message = "Overall statistics endpoint - to be implemented" });
        }

        /// <summary>
        /// Get summary for a specific student
        /// </summary>
        [HttpGet("summary/{studentId}")]
        public async Task<IActionResult> GetStudentSummary(Guid studentId)
        {
            var result = await _progressService.GetStudentSummaryAsync(studentId);
            return Ok(result);
        }
    }
}


