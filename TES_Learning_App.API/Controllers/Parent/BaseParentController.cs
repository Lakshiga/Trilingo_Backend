using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using TES_Learning_App.API.Controllers.Common;
using TES_Learning_App.Application_Layer.Common.Constants;

namespace TES_Learning_App.API.Controllers.Parent
{
    /// <summary>
    /// Base controller for all Parent controllers
    /// Routes: api/parent/*
    /// Authorization: Parent role required
    /// </summary>
    [Route("api/parent/[controller]")]
    [Authorize(Roles = RoleConstants.Parent)]
    public abstract class BaseParentController : BaseApiController
    {
        /// <summary>
        /// Gets the current parent's ID from JWT token claims
        /// </summary>
        protected Guid GetParentId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                throw new UnauthorizedAccessException("Parent ID not found in token claims");
            }
            return userId;
        }
    }
}

