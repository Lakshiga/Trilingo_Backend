using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Student.Requests;
using TES_Learning_App.Application_Layer.DTOs.Student.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, Guid parentId)
        {
            var student = new Student
            {
                Nickname = dto.Nickname,
                DateOfBirth = dto.DateOfBirth,
                Avatar = dto.Avatar,
                NativeLanguageCode = dto.NativeLanguageCode,
                TargetLanguageCode = dto.TargetLanguageCode,
                ParentId = parentId,
                IsDeleted = false,
                XpPoints = 0
            };

            object value = await _unitOfWork.StudentRepository.AddAsync(student);
            await _unitOfWork.CompleteAsync();

            return MapToStudentDto(student);
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsForParentAsync(Guid parentId)
        {
            var students = await _unitOfWork.StudentRepository.GetStudentsByParentIdAsync(parentId);
            return students.Select(MapToStudentDto);
        }

        public async Task UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, Guid parentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null || student.ParentId != parentId)
            {
                throw new Exception("Student not found or access denied."); // Will be improved with custom exceptions
            }
            student.Nickname = dto.Nickname;
            student.Avatar = dto.Avatar;
            await _unitOfWork.StudentRepository.UpdateAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(Guid studentId, Guid parentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            if (student == null || student.ParentId != parentId)
            {
                throw new Exception("Student not found or access denied.");
            }

            // --- THE SOFT DELETE LOGIC ---
            student.IsDeleted = true;
            await _unitOfWork.StudentRepository.UpdateAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        // Private helper method for mapping
        private StudentDto MapToStudentDto(Student student)
        {
            return new StudentDto
            {
                Id = student.Id,
                Nickname = student.Nickname,
                Avatar = student.Avatar,
                XpPoints = student.XpPoints,
                Age = (int)((DateTime.Now - student.DateOfBirth).TotalDays / 365.25) // The age calculation!
            };
        }
    }
}
