using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Requests;
using TES_Learning_App.Application_Layer.DTOs.MainActivity.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.Services;

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
            if (mainActivity == null) return NotFound();
            return Ok(mainActivity);
        }

        // POST: api/mainactivities - Only Admin can create
        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<MainActivityDto>> Create(CreateMainActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            try
            {
                var newMainActivity = await _mainActivityService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newMainActivity.Id }, newMainActivity);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error creating main activity: {ex.Message}" });
            }
        }

        // PUT: api/mainactivities/{id} - Only Admin can update
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Update(int id, UpdateMainActivityDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(new { isSuccess = false, message = "Validation failed", errors = ModelState });
            }

            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid main activity ID" });
            }

            try
            {
                await _mainActivityService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Main activity not found" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { isSuccess = false, message = $"Error updating main activity: {ex.Message}" });
            }
        }

        // DELETE: api/mainactivities/{id} - Only Admin can delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int id)
        {
            if (id <= 0)
            {
                return BadRequest(new { isSuccess = false, message = "Invalid main activity ID" });
            }

            try
            {
                await _mainActivityService.DeleteAsync(id);
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { isSuccess = false, message = "Main activity not found" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { isSuccess = false, message = ex.Message });
            }
        }
    }
}
