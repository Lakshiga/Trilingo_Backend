using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Parent
{
    /// <summary>
    /// Parent controller for accessing stages/lessons (Read-only)
    /// Route: api/parent/stages
    /// Authorization: Parent role required
    /// </summary>
    public class StagesController : BaseParentController
    {
        private readonly IStageService _stageService;

        public StagesController(IStageService stageService)
        {
            _stageService = stageService;
        }

        /// <summary>
        /// Get all stages (Parent - read-only)
        /// GET: api/parent/stages
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<StageDto>>> GetAll()
        {
            var stages = await _stageService.GetAllAsync();
            return Ok(stages);
        }

        /// <summary>
        /// Get stage by ID
        /// GET: api/parent/stages/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<StageDto>> GetById(int id)
        {
            var stage = await _stageService.GetByIdAsync(id);
            if (stage == null)
            {
                return NotFound(new { message = $"Stage with ID {id} not found" });
            }
            return Ok(stage);
        }

        /// <summary>
        /// Get stages by level ID
        /// GET: api/parent/stages/level/{levelId}
        /// </summary>
        [HttpGet("level/{levelId}")]
        public async Task<ActionResult<IEnumerable<StageDto>>> GetByLevelId(int levelId)
        {
            var stages = await _stageService.GetByLevelIdAsync(levelId);
            return Ok(stages);
        }
    }
}

