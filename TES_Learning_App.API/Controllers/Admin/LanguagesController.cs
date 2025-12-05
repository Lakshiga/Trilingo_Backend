using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Language.Request;
using TES_Learning_App.Application_Layer.DTOs.Language.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing languages
    /// Route: api/admin/languages
    /// Authorization: Admin role required
    /// </summary>
    public class LanguagesController : BaseAdminController
    {
        private readonly ILanguageService _languageService;

        public LanguagesController(ILanguageService languageService)
        {
            _languageService = languageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<LanguageDto>>> GetAll()
        {
            return Ok(await _languageService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<LanguageDto>> GetById(int id)
        {
            var language = await _languageService.GetByIdAsync(id);
            if (language == null) return NotFound(new { message = $"Language with ID {id} not found" });
            return Ok(language);
        }

        [HttpPost]
        public async Task<ActionResult<LanguageDto>> Create([FromBody] CreateLanguageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newLanguage = await _languageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newLanguage.Id }, newLanguage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateLanguageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _languageService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _languageService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

