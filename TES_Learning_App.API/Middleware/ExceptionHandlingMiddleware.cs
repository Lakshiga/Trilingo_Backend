using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using TES_Learning_App.Application_Layer.DTOs.Common;
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.API.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An unhandled exception occurred at {Path}", context.Request.Path);
                await HandleExceptionAsync(context, ex);
            }
        }

        private Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            ApiResponse<object> response;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    response = ApiResponse<object>.Failure(
                        validationException.Message,
                        validationException.Errors,
                        (int)code
                    );
                    break;

                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    response = ApiResponse<object>.Failure(
                        "Unauthorized access",
                        null,
                        (int)code
                    );
                    break;

                case KeyNotFoundException:
                case ArgumentNullException when exception.Message.Contains("not found"):
                    code = HttpStatusCode.NotFound;
                    response = ApiResponse<object>.Failure(
                        exception.Message,
                        null,
                        (int)code
                    );
                    break;

                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    response = ApiResponse<object>.Failure(
                        exception.Message,
                        null,
                        (int)code
                    );
                    break;

                default:
                    // In production, don't expose internal error details
                    var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";
                    var message = isDevelopment
                        ? $"An error occurred: {exception.Message}"
                        : "An error occurred while processing your request";
                    
                    response = ApiResponse<object>.Failure(
                        message,
                        null,
                        (int)code
                    );
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            var result = JsonSerializer.Serialize(response, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            return context.Response.WriteAsync(result);
        }
    }
}

