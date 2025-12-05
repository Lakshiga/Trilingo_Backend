using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.Application_Layer.DTOs.Role.Requests;
using TES_Learning_App.Application_Layer.DTOs.Role.Response;
using TES_Learning_App.Application_Layer.Interfaces.IServices;

namespace TES_Learning_App.API.Controllers.SuperAdmin
{
    /// <summary>
    /// SuperAdmin controller for managing roles
    /// Route: api/superadmin/roles
    /// Authorization: SuperAdmin role required
    /// </summary>
    public class RolesController : BaseSuperAdminController
    {
        private readonly IRoleService _roleService;

        public RolesController(IRoleService roleService)
        {
            _roleService = roleService;
        }

        /// <summary>
        /// Get all roles
        /// GET: api/superadmin/roles
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<RoleDto>>> GetAll()
        {
            var roles = await _roleService.GetAllAsync();
            return Ok(roles);
        }

        /// <summary>
        /// Get role by ID
        /// GET: api/superadmin/roles/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<RoleDto>> GetById(Guid id)
        {
            var role = await _roleService.GetByIdAsync(id);
            if (role == null)
            {
                return NotFound(new { message = $"Role with ID {id} not found" });
            }
            return Ok(role);
        }

        /// <summary>
        /// Create a new role
        /// POST: api/superadmin/roles
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<RoleDto>> Create([FromBody] CreateRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var newRole = await _roleService.CreateAsync(dto);
                return CreatedAtAction(nameof(GetById), new { id = newRole.Id }, newRole);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Update an existing role
        /// PUT: api/superadmin/roles/{id}
        /// </summary>
        [HttpPut("{id}")]
        public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto dto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _roleService.UpdateAsync(id, dto);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        /// <summary>
        /// Delete a role
        /// DELETE: api/superadmin/roles/{id}
        /// </summary>
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                await _roleService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
    }
}

