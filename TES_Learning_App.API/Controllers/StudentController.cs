using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Student.Requests;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Parent")] // This entire controller is now secure and only for Parents.
    public class StudentsController : BaseApiController
    {
        private readonly IStudentService _studentService;
        public StudentsController(IStudentService studentService) { _studentService = studentService; }

        private Guid GetParentId()
        {
            var userId = GetUserId();
            if (userId == null)
            {
                throw new UnauthorizedAccessException("User not authenticated");
            }
            return userId.Value;
        }

        [HttpPost] // POST api/students
        public async Task<IActionResult> CreateStudent(CreateStudentDto dto)
        {
            var parentId = GetParentId();
            var studentDto = await _studentService.CreateStudentAsync(dto, parentId);
            return CreatedAtAction(nameof(GetStudentById), new { studentId = studentDto.Id }, studentDto);
        }

        [HttpGet] // GET api/students
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetStudentsForParent()
        {
            var parentId = GetParentId();
            var students = await _studentService.GetStudentsForParentAsync(parentId);
            return Ok(students);
        }

        [HttpGet("{studentId}")] // GET api/students/{guid}
        public async Task<ActionResult<StudentDto>> GetStudentById(Guid studentId)
        {
            var parentId = GetParentId();
            var student = await _studentService.GetStudentByIdAsync(studentId, parentId);
            return HandleGetById(student, "Student", studentId);
        }

        [HttpPut("{studentId}")] // PUT api/students/{guid}
        public async Task<IActionResult> UpdateStudent(Guid studentId, UpdateStudentDto dto)
        {
            var parentId = GetParentId();
            await _studentService.UpdateStudentAsync(studentId, dto, parentId);
            return NoContent();
        }

        [HttpDelete("{studentId}")] // DELETE api/students/{guid}
        public async Task<IActionResult> DeleteStudent(Guid studentId)
        {
            var parentId = GetParentId();
            await _studentService.DeleteStudentAsync(studentId, parentId);
            return NoContent();
        }
    }
}
