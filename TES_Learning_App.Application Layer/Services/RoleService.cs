using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TES_Learning_App.Application_Layer.DTOs.Role.Requests;
using TES_Learning_App.Application_Layer.DTOs.Role.Response;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Domain.Entities;
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.Application_Layer.Services
{
    public class RoleService : IRoleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public RoleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public async Task<RoleDto> CreateAsync(CreateRoleDto dto)
        {
            if (string.IsNullOrWhiteSpace(dto.RoleName))
                throw new ValidationException("RoleName", new[] { "Role Name is required." });

            // Check for duplicates
            var existingRole = await _unitOfWork.RoleRepository.FindAsync(r => r.RoleName.ToLower() == dto.RoleName.ToLower());
            if (existingRole.Any())
                throw new ValidationException("RoleName", new[] { $"Role with name '{dto.RoleName}' already exists." });

            var role = new Role { RoleName = dto.RoleName };

            await _unitOfWork.RoleRepository.AddAsync(role);
            await _unitOfWork.CompleteAsync();

            return MapToDto(role);
        }

        public async Task DeleteAsync(Guid id)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            // Industrial Practice: Add a check to prevent deleting a role that is in use.
            var usersInRole = await _unitOfWork.AuthRepository.FindUsersByRoleIdAsync(id);
            if (usersInRole.Any())
            {
                throw new ValidationException("Role", new[] { $"Cannot delete role '{role.RoleName}' because it is assigned to users." });
            }
            await _unitOfWork.RoleRepository.DeleteAsync(role);
            await _unitOfWork.CompleteAsync();
        }

        public async Task<IEnumerable<RoleDto>> GetAllAsync()
        {
            var roles = await _unitOfWork.RoleRepository.GetAllAsync();
            return roles.Select(MapToDto);
        }

        public async Task<RoleDto?> GetByIdAsync(Guid id)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            return role == null ? null : MapToDto(role);
        }

        public async Task UpdateAsync(Guid id, UpdateRoleDto dto)
        {
            var role = await _unitOfWork.RoleRepository.GetByIdAsync(id);
            if (role == null) throw new KeyNotFoundException("Role not found.");

            role.RoleName = dto.RoleName;

            await _unitOfWork.RoleRepository.UpdateAsync(role);
            await _unitOfWork.CompleteAsync();
        }

        private RoleDto MapToDto(Role role)
        {
            return new RoleDto
            {
                Id = role.Id,
                RoleName = role.RoleName
            };
        }
    }
}
