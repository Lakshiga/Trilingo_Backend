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
        public async Task<ActionResult<LevelDto>> GetById(int id)
        {
            var level = await _levelService.GetByIdAsync(id);
            if (level == null)
            {
                return NotFound(); // Returns a proper 404 Not Found response
            }
            return Ok(level);
        }

        // POST: api/levels
        [HttpPost]
        public async Task<ActionResult<LevelDto>> Create(CreateLevelDto dto)
        {
            var newLevel = await _levelService.CreateAsync(dto);
            // Returns a 201 Created status with a link to the new resource
            return CreatedAtAction(nameof(GetById), new { id = newLevel.Id }, newLevel);
        }

        // PUT: api/levels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLevelDto dto)
        {
            // We can add a check here later to ensure the id in the route matches the id in the DTO if needed
            await _levelService.UpdateAsync(id, dto);
            return NoContent(); // The standard, correct response for a successful update
        }

        // DELETE: api/levels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _levelService.DeleteAsync(id);
            return NoContent(); // The standard, correct response for a successful delete
        }
    }
}
