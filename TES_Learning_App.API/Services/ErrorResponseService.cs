using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TES_Learning_App.Application_Layer.DTOs.Common;
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.API.Services
{
    public class ErrorResponseService : IErrorResponseService
    {
        public ApiResponse<T> CreateValidationErrorResponse<T>(ModelStateDictionary modelState)
        {
            var errors = ExtractModelStateErrors(modelState);
            return ApiResponse<T>.Failure(
                "Validation failed. Please check the errors and try again.",
                errors,
                400
            );
        }

        public ApiResponse<T> CreateValidationErrorResponse<T>(ValidationException exception)
        {
            return ApiResponse<T>.Failure(
                exception.Message,
                exception.Errors,
                400
            );
        }

        public ApiResponse<T> CreateErrorResponse<T>(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null)
        {
            return ApiResponse<T>.Failure(message, errors, statusCode);
        }

        public ApiResponse<T> CreateNotFoundResponse<T>(string resourceName, object? resourceId = null)
        {
            var message = resourceId != null
                ? $"{resourceName} with ID '{resourceId}' was not found."
                : $"{resourceName} was not found.";
            
            return ApiResponse<T>.Failure(message, null, 404);
        }

        public ApiResponse<T> CreateUnauthorizedResponse<T>(string message = "Unauthorized access")
        {
            return ApiResponse<T>.Failure(message, null, 401);
        }

        public ApiResponse<T> CreateInternalErrorResponse<T>(Exception exception, bool includeDetails = false)
        {
            var message = includeDetails
                ? $"An internal server error occurred: {exception.Message}"
                : "An internal server error occurred. Please try again later.";

            return ApiResponse<T>.Failure(message, null, 500);
        }

        public Dictionary<string, string[]> ExtractModelStateErrors(ModelStateDictionary modelState)
        {
            var errors = new Dictionary<string, string[]>();

            foreach (var keyValuePair in modelState)
            {
                var key = keyValuePair.Key;
                // Safety check for null
                if (keyValuePair.Value.Errors == null) continue;

                var errorMessages = keyValuePair.Value.Errors
                    .Select(e => e.ErrorMessage)
                    .Where(m => !string.IsNullOrEmpty(m))
                    .ToArray();

                if (errorMessages.Length > 0)
                {
                    errors[key] = errorMessages;
                }
            }

            return errors;
        }
    }
}


