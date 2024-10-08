﻿namespace Papyrus.Docs.AuthApi.Primitives.ApiResponse
{
    /// <summary>
    /// ApiResponse class to handle response data
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ApiResponse<T>
    {
        public ApiResponse() { }

        public ApiResponse(string message, T? data, int statusCode)
        {
            StatusCode = statusCode;
            Message = message;
            Data = data;
        }

        public ApiResponse(string message, int statusCode)
        {
            StatusCode = statusCode;
            Message = message;
        }

        public ApiResponse(string message)
        {
            Message = message;
            IsSuccess = false;
        }

        public int StatusCode { get; set; }
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
        public T? Data { get; set; }
    }
}
