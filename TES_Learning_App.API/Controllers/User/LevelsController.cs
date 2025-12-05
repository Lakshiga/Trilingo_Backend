using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Level.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing levels (Read-only)
    /// Route: api/levels
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class LevelsController : BaseUserController
    {
        private readonly ILevelService _levelService;

        public LevelsController(ILevelService levelService)
        {
            _levelService = levelService;
        }

        /// <summary>
        /// Get all levels (Mobile app - read-only)
        /// GET: api/levels
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<LevelDto>>> GetAll()
        {
            var levels = await _levelService.GetAllAsync();
            return Ok(levels);
        }

        /// <summary>
        /// Get level by ID
        /// GET: api/levels/{id}
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

