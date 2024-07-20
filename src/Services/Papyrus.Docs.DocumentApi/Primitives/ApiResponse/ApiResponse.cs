namespace Papyrus.Docs.DocumentApi.Primitives.ApiResponse
{
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
