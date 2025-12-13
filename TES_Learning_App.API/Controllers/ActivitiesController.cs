using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Activity.Requests;
using TES_Learning_App.Application_Layer.DTOs.Activity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.API.Services;

namespace TES_Learning_App.API.Controllers
{
    public class ActivitiesController : BaseApiController
    {
        private readonly IActivityService _activityService;
        private readonly IRealtimeBroadcastService _broadcastService;

        public ActivitiesController(IActivityService activityService, IRealtimeBroadcastService broadcastService)
        {
            _activityService = activityService;
            _broadcastService = broadcastService;
        }

        // GET: api/activities - Allow all authenticated users (mobile app can access)
        [HttpGet]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetAll()
        {
            return Ok(await _activityService.GetAllAsync());
        }

        // GET: api/activities/stage/5 - Allow all authenticated users
        [HttpGet("stage/{stageId}")]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<IEnumerable<ActivityDto>>> GetByStageId(int stageId)
        {
            var activities = await _activityService.GetByStageIdAsync(stageId);
            return Ok(activities);
        }

        // GET: api/activities/5 - Allow all authenticated users
        [HttpGet("{id}")]
        [AllowAnonymous] // Allow unauthenticated access for mobile app
        public async Task<ActionResult<ActivityDto>> GetById(int id)
        {
            var activity = await _activityService.GetByIdAsync(id);
            if (activity == null) return NotFound();
            return Ok(activity);
        }

        // POST: api/activities - Only Admin can create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ActivityDto>> Create(CreateActivityDto dto)
        {
            var newActivity = await _activityService.CreateAsync(dto);
            
            // Broadcast to all connected admin users
            await _broadcastService.BroadcastActivityCreatedAsync(newActivity);
            
            return CreatedAtAction(nameof(GetById), new { id = newActivity.Id }, newActivity);
        }

        // PUT: api/activities/5 - Only Admin can update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateActivityDto dto)
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

        // DELETE: api/activities/5 - Only Admin can delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
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