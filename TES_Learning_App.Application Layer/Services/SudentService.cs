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
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.Application_Layer.Services
{
    public class StudentService : IStudentService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StudentService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

        public async Task<StudentDto> CreateStudentAsync(CreateStudentDto dto, Guid parentId)
        {
            // Input validation
            if (string.IsNullOrWhiteSpace(dto.Nickname))
                throw new ValidationException("Nickname", new[] { "Nickname is required." });

            if (dto.DateOfBirth == default || dto.DateOfBirth > DateTime.Now)
                throw new ValidationException("DateOfBirth", new[] { "Valid date of birth is required." });

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

            await _unitOfWork.StudentRepository.AddAsync(student);
            await _unitOfWork.CompleteAsync();

            return MapToStudentDto(student);
        }

        public async Task<StudentDto?> GetStudentByIdAsync(Guid studentId, Guid parentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            
            if (student == null || student.IsDeleted)
            {
                return null;
            }

            if (student.ParentId != parentId)
            {
                throw new UnauthorizedAccessException("Access denied. Student does not belong to this parent.");
            }

            return MapToStudentDto(student);
        }

        public async Task<IEnumerable<StudentDto>> GetStudentsForParentAsync(Guid parentId)
        {
            // Repository already filters IsDeleted = false
            var students = await _unitOfWork.StudentRepository.GetStudentsByParentIdAsync(parentId);
            return students.Select(MapToStudentDto);
        }

        public async Task UpdateStudentAsync(Guid studentId, UpdateStudentDto dto, Guid parentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            
            if (student == null || student.IsDeleted)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            if (student.ParentId != parentId)
            {
                throw new UnauthorizedAccessException("Access denied. Student does not belong to this parent.");
            }

            // Update only provided fields
            if (!string.IsNullOrWhiteSpace(dto.Nickname))
                student.Nickname = dto.Nickname;
            
            if (!string.IsNullOrWhiteSpace(dto.Avatar))
                student.Avatar = dto.Avatar;

            await _unitOfWork.StudentRepository.UpdateAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        public async Task DeleteStudentAsync(Guid studentId, Guid parentId)
        {
            var student = await _unitOfWork.StudentRepository.GetByIdAsync(studentId);
            
            if (student == null || student.IsDeleted)
            {
                throw new KeyNotFoundException("Student not found.");
            }

            if (student.ParentId != parentId)
            {
                throw new UnauthorizedAccessException("Access denied. Student does not belong to this parent.");
            }

            // Soft delete
            student.IsDeleted = true;
            await _unitOfWork.StudentRepository.UpdateAsync(student);
            await _unitOfWork.CompleteAsync();
        }

        // Private helper method for mapping
        private StudentDto MapToStudentDto(Student student)
        {
            if (student == null)
                throw new ArgumentNullException(nameof(student));

            return new StudentDto
            {
                Id = student.Id,
                Nickname = student.Nickname,
                Avatar = student.Avatar,
                XpPoints = student.XpPoints,
                Age = student.DateOfBirth != default 
                    ? (int)((DateTime.Now - student.DateOfBirth).TotalDays / 365.25) 
                    : 0
            };
        }
    }
}
