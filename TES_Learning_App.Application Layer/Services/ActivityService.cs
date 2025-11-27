using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Activity.Requests;
using TES_Learning_App.Application_Layer.DTOs.Activity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class ActivityService : IActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public ActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<ActivityDto> CreateAsync(CreateActivityDto dto)
        {
            // We could add validation here to ensure the StageId, etc., are valid.

            var activity = new Activity
            {
                Details_JSON = dto.Details_JSON,
                StageId = dto.StageId,
                MainActivityId = dto.MainActivityId,
                ActivityTypeId = dto.ActivityTypeId,
                Name_en = dto.Name_en,
                Name_ta = dto.Name_ta,
                Name_si = dto.Name_si,
                SequenceOrder = dto.SequenceOrder
            };

            await _unitOfWork.ActivityRepository.AddAsync(activity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(activity);
        }

        public async Task DeleteAsync(int id)
        {
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(id);
            if (activity == null) throw new Exception("Activity not found.");

            try
            {
                await _unitOfWork.ActivityRepository.DeleteAsync(activity);
                await _unitOfWork.CompleteAsync();
            }
            catch (Exception ex)
            {
                // Handle database constraint violations and other errors
                if (ex.Message.Contains("foreign key") || ex.Message.Contains("constraint") || ex.Message.Contains("reference"))
                {
                    throw new Exception($"Cannot delete activity. It may be referenced by other records. Details: {ex.InnerException?.Message ?? ex.Message}");
                }
                throw new Exception($"Error deleting activity: {ex.Message}");
            }
        }

        public async Task<IEnumerable<ActivityDto>> GetAllAsync()
        {
            var activities = await _unitOfWork.ActivityRepository.GetAllAsync();
            return activities.Select(MapToDto);
        }

        public async Task<IEnumerable<ActivityDto>> GetByStageIdAsync(int stageId)
        {
            var activities = await _unitOfWork.ActivityRepository.GetByStageIdAsync(stageId);
            return activities.Select(MapToDto);
        }

        public async Task<ActivityDto?> GetByIdAsync(int id)
        {
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(id);
            return activity == null ? null : MapToDto(activity);
        }

        public async Task UpdateAsync(int id, UpdateActivityDto dto)
        {
            var activity = await _unitOfWork.ActivityRepository.GetByIdAsync(id);
            if (activity == null) throw new Exception("Activity not found.");

            // For an update, we typically only allow changing the content, not the relationships.
            if (dto.Details_JSON != null)
                activity.Details_JSON = dto.Details_JSON;
            
            if (dto.Name_en != null)
                activity.Name_en = dto.Name_en;
            
            if (dto.Name_ta != null)
                activity.Name_ta = dto.Name_ta;
            
            if (dto.Name_si != null)
                activity.Name_si = dto.Name_si;
            
            if (dto.SequenceOrder.HasValue)
                activity.SequenceOrder = dto.SequenceOrder.Value;

            await _unitOfWork.ActivityRepository.UpdateAsync(activity);
            await _unitOfWork.CompleteAsync();
        }

        private ActivityDto MapToDto(Activity activity)
        {
            return new ActivityDto
            {
                Id = activity.Id,
                Details_JSON = activity.Details_JSON,
                StageId = activity.StageId,
                MainActivityId = activity.MainActivityId,
                ActivityTypeId = activity.ActivityTypeId,
                Name_en = activity.Name_en,
                Name_ta = activity.Name_ta,
                Name_si = activity.Name_si,
                SequenceOrder = activity.SequenceOrder
            };
        }
    }
}