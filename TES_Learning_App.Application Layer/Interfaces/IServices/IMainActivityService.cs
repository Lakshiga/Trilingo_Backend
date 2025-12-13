using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Services;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface IMainActivityService
    {
        Task<IEnumerable<MainActivityDto>> GetAllAsync();
        Task<MainActivityDto?> GetByIdAsync(int id);
        Task<MainActivityDto> CreateAsync(CreateMainActivityDto dto);
        Task UpdateAsync(int id, UpdateMainActivityDto dto);
        Task DeleteAsync(int id);
    }
}
