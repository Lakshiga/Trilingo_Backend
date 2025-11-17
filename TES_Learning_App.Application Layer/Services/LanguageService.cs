using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Language.Request;
using TES_Learning_App.Application_Layer.DTOs.Language.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;

namespace TES_Learning_App.Application_Layer.Services
{
    public class LanguageService : ILanguageService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LanguageService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<LanguageDto> CreateAsync(CreateLanguageDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.LanguageName))
                throw new Exception("Language Name is required.");

            var language = new Language { LanguageName = dto.LanguageName };

            await _unitOfWork.LanguageRepository.AddAsync(language);
            await _unitOfWork.CompleteAsync();

            return MapToDto(language);
        }

        public async Task DeleteAsync(int id)
        {
            var language = await _unitOfWork.LanguageRepository.GetByIdAsync(id);
            if (language == null) throw new Exception("Language not found.");

            await _unitOfWork.LanguageRepository.DeleteAsync(language);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<LanguageDto>> GetAllAsync()
        {
            var languages = await _unitOfWork.LanguageRepository.GetAllAsync();
            return languages.Select(MapToDto);
        }

        public async Task<LanguageDto?> GetByIdAsync(int id)
        {
            var language = await _unitOfWork.LanguageRepository.GetByIdAsync(id);
            return language == null ? null : MapToDto(language);
        }

        public async Task UpdateAsync(int id, CreateLanguageDto dto)
        {
            var language = await _unitOfWork.LanguageRepository.GetByIdAsync(id);
            if (language == null) throw new Exception("Language not found.");

            language.LanguageName = dto.LanguageName;

            await _unitOfWork.LanguageRepository.DeleteAsync(language);
            await _unitOfWork.CompleteAsync();
        }

        // Private helper for consistent mapping
        private LanguageDto MapToDto(Language language)
        {
            return new LanguageDto
            {
                Id = language.Id,
                LanguageName = language.LanguageName
            };
        }
    }
}
