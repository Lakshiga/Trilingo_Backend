using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Level.Requests;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Admin,Parent")] // This entire controller is secure and for Admins only
    public class LevelsController : BaseApiController
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        // GET: api/levels
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetAll()
        {
            var levels = await _levelService.GetAllAsync();
            return Ok(levels);
        }

        // GET: api/levels/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var level = await _levelService.GetByIdAsync(id);
            return HandleGetById(level, "Level", id);
        }

        // POST: api/levels
        [HttpPost]
        public async Task<ActionResult<LevelDto>> Create(CreateLevelDto dto)
        {
            var newLevel = await _levelService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newLevel.Id }, newLevel);
        }

        // PUT: api/levels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLevelDto dto)
        {
            await _levelService.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/levels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _levelService.DeleteAsync(id);
            return NoContent();
        }
    }
}
