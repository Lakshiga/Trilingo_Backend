using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TES_Learning_App.API.Services;
using TES_Learning_App.Application_Layer.DTOs.Common;

namespace TES_Learning_App.API.Controllers
{
    /// <summary>
    /// Centralized error handling and management controller
    /// Provides standardized error responses and error management endpoints
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ErrorController : BaseApiController
    {
        private readonly IErrorResponseService _errorResponseService;
        private readonly ILogger<ErrorController> _logger;

        public ErrorController(IErrorResponseService errorResponseService, ILogger<ErrorController> logger)
        {
            _errorResponseService = errorResponseService;
            _logger = logger;
        }

        /// <summary>
        /// Test endpoint to simulate validation errors
        /// Useful for testing error handling in frontend
        /// </summary>
        [HttpPost("test-validation")]
        [AllowAnonymous]
        public IActionResult TestValidationError([FromBody] Dictionary<string, string> testData)
        {
            if (!ModelState.IsValid)
            {
                var response = _errorResponseService.CreateValidationErrorResponse<object>(ModelState);
                return BadRequest(response);
            }

            return Ok(ApiResponse<object>.Success("Validation test passed"));
        }

        /// <summary>
        /// Test endpoint to simulate not found errors
        /// </summary>
        [HttpGet("test-not-found")]
        [AllowAnonymous]
        public IActionResult TestNotFound()
        {
            var response = _errorResponseService.CreateNotFoundResponse<object>("TestResource", 999);
            return NotFound(response);
        }

        /// <summary>
        /// Test endpoint to simulate unauthorized errors
        /// </summary>
        [HttpGet("test-unauthorized")]
        [AllowAnonymous]
        public IActionResult TestUnauthorized()
        {
            var response = _errorResponseService.CreateUnauthorizedResponse<object>("Test unauthorized access");
            return Unauthorized(response);
        }

        /// <summary>
        /// Test endpoint to simulate internal server errors
        /// Only available in development
        /// </summary>
        [HttpPost("test-internal-error")]
        [AllowAnonymous]
        public IActionResult TestInternalError([FromQuery] bool includeDetails = false)
        {
            try
            {
                throw new Exception("This is a test internal server error");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Test internal error triggered");
                var response = _errorResponseService.CreateInternalErrorResponse<object>(ex, includeDetails);
                return StatusCode(500, response);
            }
        }

        /// <summary>
        /// Get error response format documentation
        /// </summary>
        [HttpGet("error-format")]
        [AllowAnonymous]
        public IActionResult GetErrorFormat()
        {
            var format = new
            {
                success = new
                {
                    isSuccess = true,
                    message = "Operation completed successfully",
                    data = new { /* your data here */ },
                    statusCode = 200
                },
                validationError = new
                {
                    isSuccess = false,
                    message = "Validation failed. Please check the errors and try again.",
                    errors = new Dictionary<string, string[]>
                    {
                        { "fieldName", new[] { "Error message 1", "Error message 2" } }
                    },
                    statusCode = 400
                },
                notFound = new
                {
                    isSuccess = false,
                    message = "Resource was not found.",
                    statusCode = 404
                },
                unauthorized = new
                {
                    isSuccess = false,
                    message = "Unauthorized access",
                    statusCode = 401
                },
                internalError = new
                {
                    isSuccess = false,
                    message = "An internal server error occurred. Please try again later.",
                    statusCode = 500
                }
            };

            return Ok(ApiResponse<object>.Success(format, "Error response format documentation"));
        }
    }
}


