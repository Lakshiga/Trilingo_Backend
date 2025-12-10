using System.Collections.Generic;

namespace TES_Learning_App.Application_Layer.DTOs.Common
{
    // Generic Response (For returning Data)
    public class ApiResponse<T>
    {
        public bool IsSuccess { get; set; }
        public string Message { get; set; } = string.Empty;
        public T? Data { get; set; }
        public Dictionary<string, string[]>? Errors { get; set; }
        public int? StatusCode { get; set; }

        public static ApiResponse<T> Success(T data, string message = "Operation completed successfully")
        {
            return new ApiResponse<T>
            {
                IsSuccess = true,
                Message = message,
                Data = data,
                StatusCode = 200
            };
        }

        public static ApiResponse<T> Failure(string message, Dictionary<string, string[]>? errors = null, int statusCode = 400)
        {
            return new ApiResponse<T>
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }

    // Non-Generic Response (For Actions without Data like Delete/Update)
    public class ApiResponse : ApiResponse<object>
    {
        // ADDED 'new' keyword to hide base member
        public new static ApiResponse Success(string message = "Operation completed successfully")
        {
            return new ApiResponse
            {
                IsSuccess = true,
                Message = message,
                StatusCode = 200
            };
        }

        public static ApiResponse Failure(string message, Dictionary<string, string[]>? errors = null, int statusCode = 400)
        {
            return new ApiResponse
            {
                IsSuccess = false,
                Message = message,
                Errors = errors,
                StatusCode = statusCode
            };
        }
    }
}