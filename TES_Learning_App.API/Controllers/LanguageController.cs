using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Language.Request;
using TES_Learning_App.Application_Layer.DTOs.Language.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Admin")] // Secure this controller for Admins
    public class LanguagesController : BaseApiController
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        // GET: api/languages
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LanguageDto>>> GetAll()
        {
            return Ok(await _languageService.GetAllAsync());
        }

        // GET: api/languages/5
        [HttpGet("{id}")]
        public async Task<ActionResult<LanguageDto>> GetById(int id)
        {
            var language = await _languageService.GetByIdAsync(id);
            if (language == null) return NotFound();
            return Ok(language);
        }

        // POST: api/languages
        [HttpPost]
        public async Task<ActionResult<LanguageDto>> Create(CreateLanguageDto dto)
        {
            var newLanguage = await _languageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newLanguage.Id }, newLanguage);
        }

        // PUT: api/languages/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, CreateLanguageDto dto)
        {
            await _languageService.UpdateAsync(id, dto);
            return NoContent(); // Standard response for a successful update
        }

        // DELETE: api/languages/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _languageService.DeleteAsync(id);
            return NoContent(); // Standard response for a successful delete
        }
    }
}
