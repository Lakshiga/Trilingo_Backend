using System;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
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
                _logger.LogError(ex, "An unhandled exception occurred");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var code = HttpStatusCode.InternalServerError;
            var result = string.Empty;

            switch (exception)
            {
                case ValidationException validationException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        message = validationException.Message,
                        errors = validationException.Errors
                    });
                    break;

                case UnauthorizedAccessException:
                    code = HttpStatusCode.Unauthorized;
                    result = JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        message = "Unauthorized access"
                    });
                    break;

                case KeyNotFoundException:
                case ArgumentNullException when exception.Message.Contains("not found"):
                    code = HttpStatusCode.NotFound;
                    result = JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        message = exception.Message
                    });
                    break;

                case ArgumentException:
                    code = HttpStatusCode.BadRequest;
                    result = JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        message = exception.Message
                    });
                    break;

                default:
                    result = JsonSerializer.Serialize(new
                    {
                        isSuccess = false,
                        message = "An error occurred while processing your request"
                    });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;

            return context.Response.WriteAsync(result);
        }
    }
}

