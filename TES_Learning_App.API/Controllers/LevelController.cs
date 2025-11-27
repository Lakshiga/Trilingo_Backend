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
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var newLevel = await _levelService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newLevel.Id }, newLevel);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error creating level: {ex.Message}" });
            }
        }

        // PUT: api/levels/5
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateLevelDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid level ID" });
            }

            try
            {
                await _levelService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Level not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error updating level: {ex.Message}" });
            }
        }

        // DELETE: api/levels/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid level ID" });
            }

            try
            {
                await _levelService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Level not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
