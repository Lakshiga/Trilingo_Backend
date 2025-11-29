using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Stage.Requests;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Admin")]
    public class StagesController : BaseApiController
    {
        private readonly IStageService _stageService;
        public StagesController(IStageService stageService) { _stageService = stageService; }
        
        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<IEnumerable<StageDto>>> GetAll() => Ok(await _stageService.GetAllAsync());
        
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<StageDto>> GetById(int id)
        {
            var stage = await _stageService.GetByIdAsync(id);
            if (stage == null) return NotFound();
            return Ok(stage);
        }
        
        // GET: api/stages/level/5 - Get all stages for a specific level
        [HttpGet("level/{levelId}")]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<IEnumerable<StageDto>>> GetByLevelId(int levelId)
        {
            var stages = await _stageService.GetByLevelIdAsync(levelId);
            return Ok(stages);
        }
        
        [HttpPost]
        public async Task<ActionResult<StageDto>> Create(CreateStageDto dto)
        {
            var newStage = await _stageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newStage.Id }, newStage);
        }
        
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStageDto dto)
        {
            await _stageService.UpdateAsync(id, dto);
            return NoContent();
        }
        
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _stageService.DeleteAsync(id);
            return NoContent();
        }
    }
}