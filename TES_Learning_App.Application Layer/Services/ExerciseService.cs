using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Requests;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Exceptions;
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
            // Validate that the Activity exists
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(dto.ActivityId);
            if (activity == null)
            {
                throw new ValidationException("ActivityId", new[] { "Activity with the specified ID does not exist" });
            }

            // Validate JSON data is valid JSON
            if (!string.IsNullOrWhiteSpace(dto.JsonData))
            {
                try
                {
                    System.Text.Json.JsonDocument.Parse(dto.JsonData);
                }
                catch
                {
                    throw new ValidationException("JsonData", new[] { "JsonData must be valid JSON format" });
                }
            }

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
            if (id <= 0)
            {
                throw new ArgumentException("Exercise ID must be greater than 0", nameof(id));
            }

            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);
            if (exercise == null)
            {
                throw new KeyNotFoundException("Exercise not found.");
            }

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
            if (id <= 0)
            {
                throw new ArgumentException("Exercise ID must be greater than 0", nameof(id));
            }

            var exercise = await _unitOfWork.ExerciseRepository.GetByIdAsync(id);
            if (exercise == null)
            {
                throw new KeyNotFoundException("Exercise not found.");
            }

            if (dto.JsonData != null)
            {
                // Validate JSON data is valid JSON
                try
                {
                    System.Text.Json.JsonDocument.Parse(dto.JsonData);
                }
                catch
                {
                    throw new ValidationException("JsonData", new[] { "JsonData must be valid JSON format" });
                }
                exercise.JsonData = dto.JsonData;
            }

            if (dto.SequenceOrder.HasValue)
            {
                if (dto.SequenceOrder.Value <= 0)
                {
                    throw new ValidationException("SequenceOrder", new[] { "SequenceOrder must be greater than 0" });
                }
                exercise.SequenceOrder = dto.SequenceOrder.Value;
            }

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
