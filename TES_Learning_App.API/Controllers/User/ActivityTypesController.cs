using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.ActivityType.Requests;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing activity types (Read-only)
    /// Route: api/activitytypes
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class ActivityTypesController : BaseUserController
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
    }
}

