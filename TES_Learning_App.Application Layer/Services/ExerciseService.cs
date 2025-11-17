using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Requests;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class ExerciseService : IExerciseService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ExerciseService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ExerciseDto> CreateAsync(CreateExerciseDto dto)
        {
            var exercise = new Exercise
            {
                ActivityId = dto.ActivityId,
                JsonData = dto.JsonData,
                SequenceOrder = dto.SequenceOrder,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _unitOfWork.ExerciseRepository.AddAsync(exercise);
            await _unitOfWork.CompleteAsync();

            return MapToDto(exercise);
        }

        public async Task DeleteAsync(int id)
        {
            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);
            if (exercise == null) throw new Exception("Exercise not found.");

            await _unitOfWork.ExerciseRepository.DeleteAsync(exercise);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ExerciseDto>> GetAllAsync()
        {
            var exercises = await _unitOfWork.ExerciseRepository.GetAllAsync();
            return exercises.Select(MapToDto);
        }

        public async Task<IEnumerable<ExerciseDto>> GetByActivityIdAsync(int activityId)
        {
            var exercises = await _unitOfWork.ExerciseRepository.GetAllAsync();
            return exercises.Where(e => e.ActivityId == activityId)
                           .OrderBy(e => e.SequenceOrder)
                           .Select(MapToDto);
        }

        public async Task<ExerciseDto?> GetByIdAsync(int id)
        {
            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);
            return exercise == null ? null : MapToDto(exercise);
        }

        public async Task UpdateAsync(int id, UpdateExerciseDto dto)
        {
            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);
            if (exercise == null) throw new Exception("Exercise not found.");

            if (dto.JsonData != null)
                exercise.JsonData = dto.JsonData;

            if (dto.SequenceOrder.HasValue)
                exercise.SequenceOrder = dto.SequenceOrder.Value;

            exercise.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.ExerciseRepository.UpdateAsync(exercise);
            await _unitOfWork.CompleteAsync();
        }

        private ExerciseDto MapToDto(Exercise exercise)
        {
            return new ExerciseDto
            {
                Id = exercise.Id,
                ActivityId = exercise.ActivityId,
                JsonData = exercise.JsonData,
                SequenceOrder = exercise.SequenceOrder,
                CreatedAt = exercise.CreatedAt,
                UpdatedAt = exercise.UpdatedAt
            };
        }
    }
}
