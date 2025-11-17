using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class MainActivityService : IMainActivityService
    {
        private readonly IUnitOfWork _unitOfWork;

        public MainActivityService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<MainActivityDto> CreateAsync(CreateMainActivityDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.Name_en))
                throw new Exception("English name is required for MainActivity.");

            var mainActivity = new MainActivity
            {
                Name_en = dto.Name_en,
                Name_ta = dto.Name_ta,
                Name_si = dto.Name_si
            };

            await _unitOfWork.MainActivityRepository.AddAsync(mainActivity);
            await _unitOfWork.CompleteAsync();

            return MapToDto(mainActivity);
        }

        public async Task DeleteAsync(int id)
        {
            var mainActivity = await _unitOfWork.MainActivityRepository.GetByIdAsync(id);
            if (mainActivity == null) throw new Exception("MainActivity not found.");

            await _unitOfWork.MainActivityRepository.DeleteAsync(mainActivity);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<MainActivityDto>> GetAllAsync()
        {
            var mainActivities = await _unitOfWork.MainActivityRepository.GetAllAsync();
            return mainActivities.Select(MapToDto);
        }

        public async Task<MainActivityDto?> GetByIdAsync(int id)
        {
            var mainActivity = await _unitOfWork.MainActivityRepository.GetByIdAsync(id);
            return mainActivity == null ? null : MapToDto(mainActivity);
        }

        public async Task UpdateAsync(int id, UpdateMainActivityDto dto)
        {
            var mainActivity = await _unitOfWork.MainActivityRepository.GetByIdAsync(id);
            if (mainActivity == null) throw new Exception("MainActivity not found.");

            mainActivity.Name_en = dto.Name_en;
            mainActivity.Name_ta = dto.Name_ta;
            mainActivity.Name_si = dto.Name_si;

            await _unitOfWork.MainActivityRepository.UpdateAsync(mainActivity);
            await _unitOfWork.CompleteAsync();
        }

        // Private helper for consistent mapping
        private MainActivityDto MapToDto(MainActivity mainActivity)
        {
            return new MainActivityDto
            {
                Id = mainActivity.Id,
                Name_en = mainActivity.Name_en,
                Name_ta = mainActivity.Name_ta,
                Name_si = mainActivity.Name_si
            };
        }
    }
}
