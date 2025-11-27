using System;
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
        public async Task<ActionResult<IEnumerable<StageDto>>> GetAll() => Ok(await _stageService.GetAllAsync());
        [HttpGet("{id}")]
        public async Task<ActionResult<StageDto>> GetById(int id)
        {
            var stage = await _stageService.GetByIdAsync(id);
            if (stage == null) return NotFound();
            return Ok(stage);
        }
        [HttpPost]
        public async Task<ActionResult<StageDto>> Create(CreateStageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var newStage = await _stageService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newStage.Id }, newStage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error creating stage: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateStageDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid stage ID" });
            }

            try
            {
                await _stageService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Stage not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error updating stage: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid stage ID" });
            }

            try
            {
                await _stageService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Stage not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
