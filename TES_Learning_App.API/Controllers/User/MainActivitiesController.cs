using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.User
{
    /// <summary>
    /// User/Mobile app controller for accessing main activities (Read-only)
    /// Route: api/mainactivities
    /// Authorization: AllowAnonymous (public access for mobile app)
    /// </summary>
    public class MainActivitiesController : BaseUserController
    {
        private readonly IMainActivityService _mainActivityService;

        public MainActivitiesController(IMainActivityService mainActivityService)
        {
            _mainActivityService = mainActivityService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MainActivityDto>>> GetAll()
        {
            return Ok(await _mainActivityService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MainActivityDto>> GetById(int id)
        {
            var mainActivity = await _mainActivityService.GetByIdAsync(id);
            if (mainActivity == null) return NotFound(new { message = $"MainActivity with ID {id} not found" });
            return Ok(mainActivity);
        }
    }
}

