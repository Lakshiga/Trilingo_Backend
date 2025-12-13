using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TES_Learning_App.Application_Layer.DTOs.Student.Request;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Parent")]
    [ApiController]
    [Route("api/[controller]")]
    public class StudentProgressController : ControllerBase
    {
        private readonly IStudentProgressService _studentProgressService;

        public StudentProgressController(IStudentProgressService studentProgressService)
        {
            _studentProgressService = studentProgressService;
        }

        private Guid GetParentId()
        {
            return Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier));
        }

        [HttpPost("submit-exercise-result/{studentId}")]
        public async Task<IActionResult> SubmitExerciseResult(Guid studentId, SubmitExerciseResultDto dto)
        {
            try
            {
                // TODO: Validate that the parent has access to this student
                // This would require injecting IStudentService or IUnitOfWork to check parent-student relationship
                var result = await _studentProgressService.SubmitExerciseResultAsync(studentId, dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
        
        // மாணவர் செயல்திறன் சுருக்கத்தைப் பெறும் எண்ட்பாயிண்ட்
        [HttpGet("{studentId}/summary")]
        public async Task<IActionResult> GetStudentProgressSummary(Guid studentId)
        {
            try
            {
                // TODO: Validate that the parent has access to this student
                var summary = await _studentProgressService.GetStudentProgressSummaryAsync(studentId);
                return Ok(summary);
            }
            catch (Exception ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }
    }
}