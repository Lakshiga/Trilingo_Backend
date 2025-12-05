using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Student.Requests;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Parent
{
    /// <summary>
    /// Parent controller for managing students
    /// Route: api/parent/students
    /// Authorization: Parent role required
    /// </summary>
    public class StudentsController : BaseParentController
    {
        private readonly IStudentService _studentService;

        public StudentsController(IStudentService studentService)
        {
            _studentService = studentService;
        }

        /// <summary>
        /// Get all students for current parent
        /// GET: api/parent/students
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAll()
        {
            var parentId = GetParentId();
            var students = await _studentService.GetStudentsForParentAsync(parentId);
            return Ok(students);
        }

        /// <summary>
        /// Get student by ID
        /// GET: api/parent/students/{studentId}
        /// </summary>
        [HttpGet("{studentId}")]
        public async Task<ActionResult<StudentDto>> GetById(Guid studentId)
        {
            var parentId = GetParentId();
            // TODO: Implement GetStudentByIdAsync method in service
            // For now, return placeholder
            return Ok(new { message = "Get by ID - To be implemented" });
        }

        /// <summary>
        /// Create a new student
        /// POST: api/parent/students
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<StudentDto>> Create([FromBody] CreateStudentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var parentId = GetParentId();
            var studentDto = await _studentService.CreateStudentAsync(dto, parentId);
            return CreatedAtAction(nameof(GetById), new { studentId = studentDto.Id }, studentDto);
        }

        /// <summary>
        /// Update an existing student
        /// PUT: api/parent/students/{studentId}
        /// </summary>
        [HttpPut("{studentId}")]
        public async Task<IActionResult> Update(Guid studentId, [FromBody] UpdateStudentDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var parentId = GetParentId();
                await _studentService.UpdateStudentAsync(studentId, dto, parentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a student
        /// DELETE: api/parent/students/{studentId}
        /// </summary>
        [HttpDelete("{studentId}")]
        public async Task<IActionResult> Delete(Guid studentId)
        {
            try
            {
                var parentId = GetParentId();
                await _studentService.DeleteStudentAsync(studentId, parentId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

