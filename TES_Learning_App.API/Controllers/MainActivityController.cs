using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    public class MainActivitiesController : BaseApiController
    {
        private readonly IMainActivityService _mainActivityService;

        public MainActivitiesController(IMainActivityService mainActivityService)
        {
            _mainActivityService = mainActivityService;
        }

        // GET: api/mainactivities - Allow mobile app to access
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<MainActivityDto>>> GetAll()
        {
            return Ok(await _mainActivityService.GetAllAsync());
        }

        // GET: api/mainactivities/{id} - Allow mobile app to access
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<MainActivityDto>> GetById(int id)
        {
            var mainActivity = await _mainActivityService.GetByIdAsync(id);
            return HandleGetById(mainActivity, "MainActivity", id);
        }

        // POST: api/mainactivities - Only Admin can create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MainActivityDto>> Create(CreateMainActivityDto dto)
        {
            var newMainActivity = await _mainActivityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newMainActivity.Id }, newMainActivity);
        }

        // PUT: api/mainactivities/{id} - Only Admin can update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateMainActivityDto dto)
        {
            await _mainActivityService.UpdateAsync(id, dto);
            return NoContent();
        }

        // DELETE: api/mainactivities/{id} - Only Admin can delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            await _mainActivityService.DeleteAsync(id);
            return NoContent();
        }
    }
}
