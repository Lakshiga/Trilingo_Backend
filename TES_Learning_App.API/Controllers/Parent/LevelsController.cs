using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Parent
{
    /// <summary>
    /// Parent controller for accessing levels (Read-only)
    /// Route: api/parent/levels
    /// Authorization: Parent role required
    /// </summary>
    public class LevelsController : BaseParentController
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        /// <summary>
        /// Get all levels (Parent - read-only)
        /// GET: api/parent/levels
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetAll()
        {
            var levels = await _levelService.GetAllAsync();
            return Ok(levels);
        }

        /// <summary>
        /// Get level by ID
        /// GET: api/parent/levels/{id}
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
    }
}

