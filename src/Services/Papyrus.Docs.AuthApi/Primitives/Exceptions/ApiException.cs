namespace Papyrus.Docs.AuthApi.Primitives.Exceptions
{
    public class ApiException(string message, int statusCode) : Exception(message)
    {
        public int StatusCode { get; set; } = statusCode;
    }
}
