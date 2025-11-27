using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Stage.Requests;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;
using TES_Learning_App.Application_Layer.Exceptions;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.Application_Layer.Services
{
    public class StageService : IStageService
    {
        private readonly IUnitOfWork _unitOfWork;
        public StageService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }
        public async Task<StageDto> CreateAsync(CreateStageDto dto)
        {
            // Validate LevelId exists
            var level = await _unitOfWork.LevelRepository.GetByIdAsync(dto.LevelId);
            if (level == null)
            {
                throw new ValidationException("LevelId", new[] { "Level with the specified ID does not exist" });
            }

            var stage = new Stage
            {
                Name_en = dto.Name_en,
                Name_ta = dto.Name_ta,
                Name_si = dto.Name_si,
                LevelId = dto.LevelId
            };
            await _unitOfWork.StageRepository.AddAsync(stage);
            await _unitOfWork.CompleteAsync();
            return MapToDto(stage);
        }

        public async Task DeleteAsync(int id)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Stage ID must be greater than 0", nameof(id));
            }

            var stage = await _unitOfWork.StageRepository.GetByIdAsync(id);
            if (stage == null)
            {
                throw new KeyNotFoundException("Stage not found.");
            }

            await _unitOfWork.StageRepository.DeleteAsync(stage);
            await _unitOfWork.CompleteAsync();
        }
        public async Task<IEnumerable<StageDto>> GetAllAsync()
        {
            var stages = await _unitOfWork.StageRepository.GetAllAsync();
            return stages.Select(MapToDto);
        }
        public async Task<StageDto?> GetByIdAsync(int id)
        {
            var stage = await _unitOfWork.StageRepository.GetByIdAsync(id);
            return stage == null ? null : MapToDto(stage);
        }
        public async Task UpdateAsync(int id, UpdateStageDto dto)
        {
            if (id <= 0)
            {
                throw new ArgumentException("Stage ID must be greater than 0", nameof(id));
            }

            var stage = await _unitOfWork.StageRepository.GetByIdAsync(id);
            if (stage == null)
            {
                throw new KeyNotFoundException("Stage not found.");
            }

            // Update only if provided
            if (dto.Name_en != null)
                stage.Name_en = dto.Name_en;
            if (dto.Name_ta != null)
                stage.Name_ta = dto.Name_ta;
            if (dto.Name_si != null)
                stage.Name_si = dto.Name_si;

            await _unitOfWork.StageRepository.UpdateAsync(stage);
            await _unitOfWork.CompleteAsync();
        }
        private StageDto MapToDto(Stage stage)
        {
            return new StageDto
            {
                Id = stage.Id,
                Name_en = stage.Name_en,
                Name_ta = stage.Name_ta,
                Name_si = stage.Name_si,
                LevelId = stage.LevelId
            };
        }
    }
}
