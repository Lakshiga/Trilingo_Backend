using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Parent
{
    /// <summary>
    /// Parent controller for accessing exercises (Read-only)
    /// Route: api/parent/exercises
    /// Authorization: Parent role required
    /// </summary>
    public class ExercisesController : BaseParentController
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        /// <summary>
        /// Get all exercises (Parent - read-only)
        /// GET: api/parent/exercises
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return Ok(exercises);
        }

        /// <summary>
        /// Get exercise by ID
        /// GET: api/parent/exercises/{id}
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
        /// GET: api/parent/activities/{activityId}/exercises
        /// </summary>
        [HttpGet("/api/parent/activities/{activityId}/exercises")]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByActivityId(int activityId)
        {
            var exercises = await _exerciseService.GetByActivityIdAsync(activityId);
            return Ok(exercises);
        }
    }
}

