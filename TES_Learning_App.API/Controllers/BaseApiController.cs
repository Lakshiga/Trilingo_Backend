using System;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using TES_Learning_App.API.Services;
using TES_Learning_App.Application_Layer.DTOs.Common;

namespace TES_Learning_App.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public abstract class BaseApiController : ControllerBase
    {
        private IErrorResponseService? _errorResponseService;
        
        /// <summary>
        /// Lazy-loaded ErrorResponseService to avoid repeating dependency injection in every controller
        /// </summary>
        protected IErrorResponseService ErrorResponseService =>
            _errorResponseService ??= HttpContext.RequestServices.GetRequiredService<IErrorResponseService>();

        /// <summary>
        /// Validates ModelState and returns BadRequest if invalid - eliminates repeated validation code
        /// </summary>
        protected IActionResult? ValidateModelState<T>()
        {
            if (!ModelState.IsValid)
            {
                var errorResponse = ErrorResponseService.CreateValidationErrorResponse<T>(ModelState);
                return BadRequest(errorResponse);
            }
            return null;
        }

        /// <summary>
        /// Validates ID is greater than 0 - eliminates repeated ID validation code
        /// </summary>
        protected IActionResult? ValidateId(int id, string resourceName = "Resource")
        {
            if (id <= 0)
            {
                var errorResponse = ErrorResponseService.CreateErrorResponse<object>(
                    $"Invalid {resourceName} ID", 
                    400
                );
                return BadRequest(errorResponse);
            }
            return null;
        }

        /// <summary>
        /// Handles GetById pattern - eliminates repeated null check code
        /// </summary>
        protected IActionResult HandleGetById<T>(T? result, string resourceName, object? resourceId = null)
        {
            if (result == null)
            {
                var errorResponse = ErrorResponseService.CreateNotFoundResponse<T>(resourceName, resourceId);
                return NotFound(errorResponse);
            }
            return Ok(result);
        }

        /// <summary>
        /// Handles exceptions consistently - eliminates repeated try-catch code
        /// </summary>
        protected IActionResult HandleException<T>(Exception ex)
        {
            var errorResponse = ErrorResponseService.CreateInternalErrorResponse<T>(ex);
            return StatusCode(500, errorResponse);
        }

        /// <summary>
        /// Handles KeyNotFoundException - eliminates repeated NotFound handling
        /// </summary>
        protected IActionResult HandleNotFound<T>(string resourceName, object? resourceId = null)
        {
            var errorResponse = ErrorResponseService.CreateNotFoundResponse<T>(resourceName, resourceId);
            return NotFound(errorResponse);
        }

        /// <summary>
        /// Handles service result with IsSuccess check - eliminates repeated result checking
        /// </summary>
        protected IActionResult HandleServiceResult<T>(T result) where T : class
        {
            // Check if result has IsSuccess property using reflection
            var isSuccessProperty = typeof(T).GetProperty("IsSuccess");
            if (isSuccessProperty != null)
            {
                var isSuccess = (bool)(isSuccessProperty.GetValue(result) ?? false);
                if (!isSuccess)
                {
                    var messageProperty = typeof(T).GetProperty("Message");
                    var message = messageProperty?.GetValue(result)?.ToString() ?? "Operation failed";
                    var errorResponse = ErrorResponseService.CreateErrorResponse<T>(message, 400);
                    return BadRequest(errorResponse);
                }
            }
            return Ok(result);
        }

        /// <summary>
        /// Gets the current user's ID from claims - eliminates repeated claim extraction
        /// </summary>
        protected Guid? GetUserId()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier);
            return Guid.TryParse(userIdClaim, out var userId) ? userId : null;
        }

        /// <summary>
        /// Gets the current user's username from claims - eliminates repeated claim extraction
        /// </summary>
        protected string? GetUsername()
        {
            return User.Identity?.Name;
        }

        /// <summary>
        /// Gets the current user's role from claims - eliminates repeated claim extraction
        /// </summary>
        protected string? GetUserRole()
        {
            return User.FindFirstValue(ClaimTypes.Role);
        }
    }
}
