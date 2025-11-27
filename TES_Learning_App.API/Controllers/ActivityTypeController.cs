using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "Admin,Parent")]
    public class ActivityTypesController : BaseApiController
    {
        private readonly IActivityTypeService _activityTypeService;

        public ActivityTypesController(IActivityTypeService activityTypeService)
        {
            _activityTypeService = activityTypeService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityTypeDto>>> GetAll()
        {
            return Ok(await _activityTypeService.GetAllAsync());
        }

        [HttpGet("by-main-activity/{mainActivityId}")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ActivityTypeDto>>> GetByMainActivity(int mainActivityId)
        {
            var activityTypes = await _activityTypeService.GetByMainActivityAsync(mainActivityId);
            return Ok(activityTypes);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityTypeDto>> GetById(int id)
        {
            var activityType = await _activityTypeService.GetByIdAsync(id);
            if (activityType == null) return NotFound();
            return Ok(activityType);
        }

        [HttpPost]
        public async Task<ActionResult<ActivityTypeDto>> Create(CreateActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var newActivityType = await _activityTypeService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newActivityType.Id }, newActivityType);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error creating activity type: {ex.Message}" });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, UpdateActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid activity type ID" });
            }

            try
            {
                await _activityTypeService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Activity type not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error updating activity type: {ex.Message}" });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid activity type ID" });
            }

            try
            {
                await _activityTypeService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Activity type not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
