using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Level.Requests;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing levels
    /// Route: api/admin/levels
    /// Authorization: Admin role required
    /// </summary>
    public class LevelsController : BaseAdminController
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        /// <summary>
        /// Get all levels
        /// GET: api/admin/levels
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetAll()
        {
            var levels = await _levelService.GetAllAsync();
            return Ok(levels);
        }

        /// <summary>
        /// Get level by ID
        /// GET: api/admin/levels/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<LevelDto>> GetById(int id)
        {
            var level = await _levelService.GetByIdAsync(id);
            if (level == null)
            {
                return NotFound(new { message = $"Level with ID {id} not found" });
            }
            return Ok(level);
        }

        /// <summary>
        /// Create a new level
        /// POST: api/admin/levels
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<LevelDto>> Create([FromBody] CreateLevelDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newLevel = await _levelService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newLevel.Id }, newLevel);
        }

        /// <summary>
        /// Update an existing level
        /// PUT: api/admin/levels/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateLevelDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _levelService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a level
        /// DELETE: api/admin/levels/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _levelService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

