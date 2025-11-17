using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class ActivityTypeService : IActivityTypeService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityTypeService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivityTypeDto> CreateAsync(CreateActivityTypeDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name_en))
                throw new Exception("English name is required for ActivityType.");

            var activityType = new ActivityType
            {
                Name_en = dto.Name_en,
                Name_ta = dto.Name_ta,
                Name_si = dto.Name_si,
                JsonMethod = dto.JsonMethod
            };

            await _unitOfWork.ActivityTypeRepository.AddAsync(activityType);
            await _unitOfWork.CompleteAsync();

            return MapToDto(activityType);
        }

        public async Task DeleteAsync(int id)
        {
            var activityType = await _unitOfWork.ActivityTypeRepository.GetByIdAsync(id);
            if (activityType == null) throw new Exception("ActivityType not found.");

            await _unitOfWork.ActivityTypeRepository.DeleteAsync(activityType);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<ActivityTypeDto>> GetAllAsync()
        {
            var activityTypes = await _unitOfWork.ActivityTypeRepository.GetAllAsync();
            return activityTypes.Select(MapToDto);
        }

        public async Task<ActivityTypeDto?> GetByIdAsync(int id)
        {
            var activityType = await _unitOfWork.ActivityTypeRepository.GetByIdAsync(id);
            return activityType == null ? null : MapToDto(activityType);
        }

        public async Task UpdateAsync(int id, UpdateActivityTypeDto dto)
        {
            var activityType = await _unitOfWork.ActivityTypeRepository.GetByIdAsync(id);
            if (activityType == null) throw new Exception("ActivityType not found.");

            activityType.Name_en = dto.Name_en;
            activityType.Name_ta = dto.Name_ta;
            activityType.Name_si = dto.Name_si;
            if (dto.JsonMethod != null)
            {
                activityType.JsonMethod = dto.JsonMethod;
            }

            await _unitOfWork.ActivityTypeRepository.UpdateAsync(activityType);
            await _unitOfWork.CompleteAsync();
        }

        private ActivityTypeDto MapToDto(ActivityType activityType)
        {
            return new ActivityTypeDto
            {
                Id = activityType.Id,
                Name_en = activityType.Name_en,
                Name_ta = activityType.Name_ta,
                Name_si = activityType.Name_si,
                JsonMethod = activityType.JsonMethod
            };
        }
    }
}
