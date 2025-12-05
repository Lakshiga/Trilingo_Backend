using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Services;
using TES_Learning_App.Application_Layer.DTOs.Activity.Requests;
using TES_Learning_App.Application_Layer.DTOs.Activity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing activities
    /// Route: api/admin/activities
    /// Authorization: Admin role required
    /// </summary>
    public class ActivitiesController : BaseAdminController
    {
        private readonly IActivityService _activityService;
        private readonly IRealtimeBroadcastService _broadcastService;

        public ActivitiesController(
            IActivityService activityService, 
            IRealtimeBroadcastService broadcastService)
        {
            _activityService = activityService;
            _broadcastService = broadcastService;
        }

        /// <summary>
        /// Get all activities (Admin view with full details)
        /// GET: api/admin/activities
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAll()
        {
            var activities = await _activityService.GetAllAsync();
            return Ok(activities);
        }

        /// <summary>
        /// Get activity by ID
        /// GET: api/admin/activities/{id}
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
        /// GET: api/admin/activities/stage/{stageId}
        /// </summary>
        [HttpGet("stage/{stageId}")]
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetByStageId(int stageId)
        {
            var activities = await _activityService.GetByStageIdAsync(stageId);
            return Ok(activities);
        }

        /// <summary>
        /// Create a new activity
        /// POST: api/admin/activities
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ActivityDto>> Create([FromBody] CreateActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newActivity = await _activityService.CreateAsync(dto);

            // Broadcast to all connected admin users
            await _broadcastService.BroadcastActivityCreatedAsync(newActivity);

            return CreatedAtAction(
                nameof(GetById), 
                new { id = newActivity.Id }, 
                newActivity);
        }

        /// <summary>
        /// Update an existing activity
        /// PUT: api/admin/activities/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _activityService.UpdateAsync(id, dto);

                // Get updated activity and broadcast
                var updatedActivity = await _activityService.GetByIdAsync(id);
                if (updatedActivity != null)
                {
                    await _broadcastService.BroadcastActivityUpdatedAsync(updatedActivity);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an activity
        /// DELETE: api/admin/activities/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _activityService.DeleteAsync(id);

                // Broadcast deletion to all connected admin users
                await _broadcastService.BroadcastActivityDeletedAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

