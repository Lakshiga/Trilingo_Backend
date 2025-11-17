using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Language.Request;
using TES_Learning_App.Application_Layer.DTOs.Language.Response;

namespace TES_Learning_App.Application_Layer.Interfaces.IServices
{
    public interface ILanguageService
    {
        Task<IEnumerable<LanguageDto>> GetAllAsync();
        Task<LanguageDto?> GetByIdAsync(int id);
        Task<LanguageDto> CreateAsync(CreateLanguageDto dto);
        Task UpdateAsync(int id, CreateLanguageDto dto); // Using CreateLanguageDto for update is simple for now
        Task DeleteAsync(int id);
    }
}
