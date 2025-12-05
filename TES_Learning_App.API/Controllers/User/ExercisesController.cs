using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Exercise.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing exercises (Read-only)
    /// Route: api/exercises
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class ExercisesController : BaseUserController
    {
        private readonly IExerciseService _exerciseService;

        public ExercisesController(IExerciseService exerciseService)
        {
            _exerciseService = exerciseService;
        }

        /// <summary>
        /// Get all exercises (Mobile app - read-only)
        /// GET: api/exercises
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetAll()
        {
            var exercises = await _exerciseService.GetAllAsync();
            return Ok(exercises);
        }

        /// <summary>
        /// Get exercise by ID
        /// GET: api/exercises/{id}
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
        /// GET: api/activities/{activityId}/exercises
        /// </summary>
        [HttpGet("/api/activities/{activityId}/exercises")]
        public async Task<ActionResult<IEnumerable<ExerciseDto>>> GetByActivityId(int activityId)
        {
            var exercises = await _exerciseService.GetByActivityIdAsync(activityId);
            return Ok(exercises);
        }
    }
}

