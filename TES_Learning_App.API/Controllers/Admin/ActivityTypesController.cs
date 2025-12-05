using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing activity types
    /// Route: api/admin/activitytypes
    /// Authorization: Admin role required
    /// </summary>
    public class ActivityTypesController : BaseAdminController
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

        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityTypeDto>> GetById(int id)
        {
            var activityType = await _activityTypeService.GetByIdAsync(id);
            if (activityType == null) return NotFound(new { message = $"ActivityType with ID {id} not found" });
            return Ok(activityType);
        }

        [HttpGet("by-main-activity/{mainActivityId}")]
        public async Task<ActionResult<IEnumerable<ActivityTypeDto>>> GetByMainActivity(int mainActivityId)
        {
            var activityTypes = await _activityTypeService.GetByMainActivityAsync(mainActivityId);
            return Ok(activityTypes);
        }

        [HttpPost]
        public async Task<ActionResult<ActivityTypeDto>> Create([FromBody] CreateActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newActivityType = await _activityTypeService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newActivityType.Id }, newActivityType);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateActivityTypeDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _activityTypeService.UpdateAsync(id, dto);
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
                await _activityTypeService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

