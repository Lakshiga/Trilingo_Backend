using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Activity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing activities (Read-only)
    /// Route: api/activities
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class ActivitiesController : BaseUserController
    {
        private readonly IActivityService _activityService;

        public ActivitiesController(IActivityService activityService)
        {
            _activityService = activityService;
        }

        /// <summary>
        /// Get all activities (Mobile app - read-only)
        /// GET: api/activities
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAll()
        {
            var activities = await _activityService.GetAllAsync();
            return Ok(activities);
        }

        /// <summary>
        /// Get activity by ID
        /// GET: api/activities/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ActivityDto>> GetById(int id)
        {
            var activity = await _activityService.GetByIdAsync(id);
            if (activity == null)
            {
                return NotFound(new { message = $"Activity with ID {id} not found" });
            }
            return Ok(activity);
        }

        /// <summary>
        /// Get activities by stage ID
        /// GET: api/activities/stage/{stageId}
        /// </summary>
        [HttpGet("stage/{stageId}")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetByStageId(int stageId)
        {
            var activities = await _activityService.GetByStageIdAsync(stageId);
            return Ok(activities);
        }
    }
}

