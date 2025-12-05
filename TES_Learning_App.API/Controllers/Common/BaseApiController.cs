using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace TES_Learning_App.API.Controllers.Common
{
    /// <summary>
    /// Base controller for all API controllers with common functionality
    /// </summary>
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        /// <summary>
        /// Gets the current user's ID from JWT token claims
        /// </summary>
        protected Guid? GetCurrentUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Gets the current user's username from JWT token claims
        /// </summary>
        protected string? GetCurrentUsername()
        {
            return User.Identity?.Name;
        }

        /// <summary>
        /// Gets the current user's role from JWT token claims
        /// </summary>
        protected string? GetCurrentUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role);
        }

        /// <summary>
        /// Checks if current user has a specific role
        /// </summary>
        protected bool HasRole(string role)
        {
            return User.IsInRole(role);
        }
    }
}

