using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Progress.Requests;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Parent")]
    public class StudentProgressController : BaseApiController
    {
        private readonly IStudentProgressService _progressService;

        public StudentProgressController(IStudentProgressService progressService)
        {
            _progressService = progressService;
        }

        /// <summary>
        /// Get progress for all children of the authenticated parent
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetProgress([FromQuery] ProgressFilterDto filter)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.GetFilteredProgressAsync(filter, parentId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Get progress for a specific student (must be child of authenticated parent)
        /// </summary>
        [HttpGet("{studentId}")]
        public async Task<IActionResult> GetStudentProgress(Guid studentId)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.GetStudentProgressAsync(studentId, parentId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Get detailed statistics for a student
        /// </summary>
        [HttpGet("{studentId}/stats")]
        public async Task<IActionResult> GetStudentStats(Guid studentId)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.GetStudentStatsAsync(studentId, parentId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Get summary for a student
        /// </summary>
        [HttpGet("{studentId}/summary")]
        public async Task<IActionResult> GetStudentSummary(Guid studentId)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.GetStudentSummaryAsync(studentId, parentId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Create a new progress record
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CreateProgress([FromBody] CreateProgressDto dto)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.CreateProgressAsync(dto, parentId.Value);
            return CreatedAtAction(nameof(GetProgress), new { id = result.Id }, result);
        }

        /// <summary>
        /// Update an existing progress record
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateProgress(int id, [FromBody] UpdateProgressDto dto)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.UpdateProgressAsync(id, dto, parentId.Value);
            return Ok(result);
        }

        /// <summary>
        /// Get a specific progress record by ID
        /// </summary>
        [HttpGet("record/{id}")]
        public async Task<IActionResult> GetProgressById(int id)
        {
            var parentId = GetUserId();
            if (!parentId.HasValue)
            {
                var errorResponse = ErrorResponseService.CreateUnauthorizedResponse<object>("User not authenticated");
                return Unauthorized(errorResponse);
            }

            var result = await _progressService.GetProgressByIdAsync(id, parentId.Value);
            return HandleGetById(result, "Progress", id);
        }
    }
}


