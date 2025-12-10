using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Student.Requests;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IStudentService
    {
        Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, Guid parentId);
        Task<StudentDto?> GetStudentByIdAsync(Guid studentId, Guid parentId);
        Task<IEnumerable<StudentDto>> GetStudentsForParentAsync(Guid parentId);
        Task UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, Guid parentId);
        Task DeleteStudentAsync(Guid studentId, Guid parentId);
    }
}
