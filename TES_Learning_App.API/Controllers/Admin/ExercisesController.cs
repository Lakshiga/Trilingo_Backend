using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Services;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Requests;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing exercises
    /// Route: api/admin/exercises
    /// Authorization: Admin role required
    /// </summary>
    public class ExercisesController : BaseAdminController
    {
        private readonly IExerciseService _exerciseService;
        private readonly IRealtimeBroadcastService _broadcastService;

        public ExercisesController(
            IExerciseService exerciseService,
            IRealtimeBroadcastService broadcastService)
        {
            _exerciseService = exerciseService;
            _broadcastService = broadcastService;
        }

        /// <summary>
        /// Get all exercises (Admin view with full details)
        /// GET: api/admin/exercises
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return Ok(exercises);
        }

        /// <summary>
        /// Get exercise by ID
        /// GET: api/admin/exercises/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ExerciseDto>> GetById(int id)
        {
            var exercise = await _exerciseService.GetByIdAsync(id);
            if (exercise == null)
            {
                return NotFound(new { message = $"Exercise with ID {id} not found" });
            }
            return Ok(exercise);
        }

        /// <summary>
        /// Get exercises by activity ID
        /// GET: api/admin/exercises/activity/{activityId}
        /// </summary>
        [HttpGet("activity/{activityId}")]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByActivityId(int activityId)
        {
            var exercises = await _exerciseService.GetByActivityIdAsync(activityId);
            return Ok(exercises);
        }

        /// <summary>
        /// Create a new exercise
        /// POST: api/admin/exercises
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<ExerciseDto>> Create([FromBody] CreateExerciseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newExercise = await _exerciseService.CreateAsync(dto);

            // Broadcast to all connected admin users
            await _broadcastService.BroadcastExerciseCreatedAsync(newExercise);

            return CreatedAtAction(
                nameof(GetById),
                new { id = newExercise.Id },
                newExercise);
        }

        /// <summary>
        /// Update an existing exercise
        /// PUT: api/admin/exercises/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateExerciseDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _exerciseService.UpdateAsync(id, dto);

                // Get updated exercise and broadcast
                var updatedExercise = await _exerciseService.GetByIdAsync(id);
                if (updatedExercise != null)
                {
                    await _broadcastService.BroadcastExerciseUpdatedAsync(updatedExercise);
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete an exercise
        /// DELETE: api/admin/exercises/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _exerciseService.DeleteAsync(id);

                // Broadcast deletion to all connected admin users
                await _broadcastService.BroadcastExerciseDeletedAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

