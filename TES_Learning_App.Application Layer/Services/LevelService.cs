using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Level.Requests;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;
using TES_Learning_App.Application_Layer.Exceptions;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
namespace TES_Learning_App.Application_Layer.Services

{
    public class LevelService : ILevelService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LevelService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LevelDto> CreateAsync(CreateLevelDto dto)
        {
            // Validate LanguageId exists
            var language = await _unitOfWork.LanguageRepository.GetByIdAsync(dto.LanguageId);
            if (language == null)
            {
                throw new ValidationException("LanguageId", new[] { "Language with the specified ID does not exist" });
            }

            // Map the DTO to our Domain Entity
            var level = new Level
            {
                Name_en = dto.Name_en,
                Name_ta = dto.Name_ta,
                Name_si = dto.Name_si,
                LanguageId = dto.LanguageId
            };

            // Use the repository to add the entity and the Unit of Work to save it
            await _unitOfWork.LevelRepository.AddAsync(level);
            await _unitOfWork.CompleteAsync();

            // Map the result back to a DTO to return to the user
            return MapToDto(level);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Level ID must be greater than 0", nameof(id));
            }

            var level = await _unitOfWork.LevelRepository.GetByIdAsync(id);
            if (level == null)
            {
                throw new KeyNotFoundException("Level not found.");
            }

            await _unitOfWork.LevelRepository.DeleteAsync(level);
            await _unitOfWork.CompleteAsync();
        }


        public async Task<IEnumerable<LevelDto>> GetAllAsync()
        {
            var levels = await _unitOfWork.LevelRepository.GetAllAsync();
            return levels.Select(MapToDto);
        }

        public async Task<LevelDto?> GetByIdAsync(int id)
        {
            var level = await _unitOfWork.LevelRepository.GetByIdAsync(id);
            return level == null ? null : MapToDto(level);
        }

        public async Task UpdateAsync(int id, UpdateLevelDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Level ID must be greater than 0", nameof(id));
            }

            var level = await _unitOfWork.LevelRepository.GetByIdAsync(id);
            if (level == null)
            {
                throw new KeyNotFoundException("Level not found.");
            }

            // Update the properties from the DTO (only if provided)
            if (dto.Name_en != null)
                level.Name_en = dto.Name_en;
            if (dto.Name_ta != null)
                level.Name_ta = dto.Name_ta;
            if (dto.Name_si != null)
                level.Name_si = dto.Name_si;

            await _unitOfWork.LevelRepository.UpdateAsync(level);
            await _unitOfWork.CompleteAsync();
        }

        // This private helper keeps our mapping logic consistent and in one place.
        private LevelDto MapToDto(Level level)
        {
            return new LevelDto
            {
                Id = level.Id,
                Name_en = level.Name_en,
                Name_ta = level.Name_ta,
                Name_si = level.Name_si,
                LanguageId = level.LanguageId
            };
        }
    }
}
