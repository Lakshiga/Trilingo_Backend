using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.Admin
{
    /// <summary>
    /// Admin controller for managing main activities
    /// Route: api/admin/mainactivities
    /// Authorization: Admin role required
    /// </summary>
    public class MainActivitiesController : BaseAdminController
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

        [HttpPost]
        public async Task<ActionResult<MainActivityDto>> Create([FromBody] CreateMainActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var newMainActivity = await _mainActivityService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newMainActivity.Id }, newMainActivity);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateMainActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _mainActivityService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                await _mainActivityService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

