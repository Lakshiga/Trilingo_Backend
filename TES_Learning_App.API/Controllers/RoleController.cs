using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Role.Requests;
using TES_Learning_App.Application_Layer.DTOs.Role.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers
{
    [Authorize(Roles = "SuperAdmin")] // Use a more restrictive role for safety
    public class RolesController : BaseApiController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            return Ok(await _roleService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null) return NotFound();
            return Ok(role);
        }

        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create(CreateRoleDto dto)
        {
            var newRole = await _roleService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, UpdateRoleDto dto)
        {
            await _roleService.UpdateAsync(id, dto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            await _roleService.DeleteAsync(id);
            return NoContent();
        }
    }
}
