using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Admin.Requests;
using TES_Learning_App.Application_Layer.DTOs.Admin.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "SuperAdmin")]
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseApiController
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        /// <summary>
        /// Get all admins (SuperAdmin only)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<AdminListDto>>> GetAllAdmins()
        {
            var admins = await _adminService.GetAllAdminsAsync();
            return Ok(admins);
        }

        /// <summary>
        /// Get admin by ID (SuperAdmin only)
        /// </summary>
        [HttpGet("{adminId}")]
        public async Task<ActionResult<AdminDto>> GetAdminById(Guid adminId)
        {
            var admin = await _adminService.GetAdminByIdAsync(adminId);
            return HandleGetById(admin, "Admin", adminId);
        }

        /// <summary>
        /// Create new admin user (SuperAdmin only)
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<AdminDto>> CreateAdmin([FromBody] CreateAdminDto dto)
        {
            var admin = await _adminService.CreateAdminAsync(dto);
            return CreatedAtAction(nameof(GetAdminById), new { adminId = admin.Id }, admin);
        }

        /// <summary>
        /// Update admin information (SuperAdmin only)
        /// </summary>
        [HttpPut("{adminId}")]
        public async Task<ActionResult<AdminDto>> UpdateAdmin(Guid adminId, [FromBody] UpdateAdminDto dto)
        {
            var admin = await _adminService.UpdateAdminAsync(adminId, dto);
            return Ok(admin);
        }

        /// <summary>
        /// Delete admin user (SuperAdmin only)
        /// Prevents deletion of last SuperAdmin
        /// </summary>
        [HttpDelete("{adminId}")]
        public async Task<IActionResult> DeleteAdmin(Guid adminId)
        {
            await _adminService.DeleteAdminAsync(adminId);
            return NoContent();
        }

        /// <summary>
        /// Reset admin password (SuperAdmin only)
        /// </summary>
        [HttpPost("{adminId}/reset-password")]
        public async Task<ActionResult<AdminDto>> ResetAdminPassword(Guid adminId, [FromBody] ResetAdminPasswordDto dto)
        {
            var admin = await _adminService.ResetAdminPasswordAsync(adminId, dto);
            return Ok(admin);
        }
    }
}

