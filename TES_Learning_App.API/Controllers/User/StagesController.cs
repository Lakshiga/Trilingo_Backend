using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing stages (Read-only)
    /// Route: api/stages
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class StagesController : BaseUserController
    {
        private readonly IStageService _stageService;

        public StagesController(IStageService stageService)
        {
            _stageService = stageService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<StageDto>>> GetAll()
        {
            return Ok(await _stageService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<StageDto>> GetById(int id)
        {
            var stage = await _stageService.GetByIdAsync(id);
            if (stage == null) return NotFound(new { message = $"Stage with ID {id} not found" });
            return Ok(stage);
        }

        [HttpGet("level/{levelId}")]
        public async Task<ActionResult<IEnumerable<StageDto>>> GetByLevelId(int levelId)
        {
            var stages = await _stageService.GetByLevelIdAsync(levelId);
            return Ok(stages);
        }
    }
}

