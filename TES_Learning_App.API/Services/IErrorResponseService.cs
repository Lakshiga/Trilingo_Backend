using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using TES_Learning_App.Application_Layer.DTOs.Common;
using TES_Learning_App.Application_Layer.Exceptions;

namespace TES_Learning_App.API.Services
{
    public interface IErrorResponseService
    {
        ApiResponse<T> CreateValidationErrorResponse<T>(ModelStateDictionary modelState);
        ApiResponse<T> CreateValidationErrorResponse<T>(ValidationException exception);
        ApiResponse<T> CreateErrorResponse<T>(string message, int statusCode = 400, Dictionary<string, string[]>? errors = null);
        ApiResponse<T> CreateNotFoundResponse<T>(string resourceName, object? resourceId = null);
        ApiResponse<T> CreateUnauthorizedResponse<T>(string message = "Unauthorized access");
        ApiResponse<T> CreateInternalErrorResponse<T>(Exception exception, bool includeDetails = false);
        Dictionary<string, string[]> ExtractModelStateErrors(ModelStateDictionary modelState);
    }
}


