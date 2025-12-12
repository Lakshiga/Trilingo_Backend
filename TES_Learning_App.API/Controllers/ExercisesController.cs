using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Requests;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.API.Services;

namespace TES_Learning_App.API.Controllers
{
    public class ExercisesController : BaseApiController
    {
        private readonly IExerciseService _exerciseService;
        private readonly IRealtimeBroadcastService _broadcastService;

        public ExercisesController(IExerciseService exerciseService, IRealtimeBroadcastService broadcastService)
        {
            _exerciseService = exerciseService;
            _broadcastService = broadcastService;
        }

        // GET: api/exercises - Allow mobile app to access
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
        {
            return Ok(await _exerciseService.GetAllAsync());
        }

        // GET: api/activities/{activityId}/exercises - Allow mobile app to access
        [HttpGet("/api/activities/{activityId}/exercises")]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByActivityId(int activityId)
        {
            var exercises = await _exerciseService.GetByActivityIdAsync(activityId);
            return Ok(exercises);
        }

        // GET: api/exercises/5 - Allow mobile app to access
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var exercise = await _exerciseService.GetByIdAsync(id);
            return HandleGetById(exercise, "Exercise", id);
        }

        // POST: api/exercises - Only Admin can create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ExerciseDto>> Create(CreateExerciseDto dto)
        {
            var newExercise = await _exerciseService.CreateAsync(dto);
            
            // Broadcast to all connected admin users
            await _broadcastService.BroadcastExerciseCreatedAsync(newExercise);
            
            return CreatedAtAction(nameof(GetById), new { id = newExercise.Id }, newExercise);
        }

        // PUT: api/exercises/5 - Only Admin can update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateExerciseDto dto)
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

        // DELETE: api/exercises/5 - Only Admin can delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _exerciseService.DeleteAsync(id);
            
            // Broadcast deletion to all connected admin users
            await _broadcastService.BroadcastExerciseDeletedAsync(id);
            
            return NoContent();
        }
    }
}
