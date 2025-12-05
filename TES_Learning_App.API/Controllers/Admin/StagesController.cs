using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Stage.Requests;
using TES_Learning_App.Application_Layer.DTOs.Stage.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing stages
    /// Route: api/admin/stages
    /// Authorization: Admin role required
    /// </summary>
    public class StagesController : BaseAdminController
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

        [HttpPost]
        public async Task<ActionResult<StageDto>> Create([FromBody] CreateStageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newStage = await _stageService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newStage.Id }, newStage);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateStageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _stageService.UpdateAsync(id, dto);
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
                await _stageService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

