using Papyrus.Docs.AuthApi.Primitives.Exceptions;
using System.Net;
using System.Text.Json;

namespace Papyrus.Docs.AuthApi.Middleware
{
    /// <summary>
    /// Middleware to handle exceptions
    /// </summary>
    /// <param name="next"> The next request delegate to call </param>
    public class ErrorHanldeMiddleware(RequestDelegate next)
    {
        /// <summary>
        /// Invoke the middleware to handle exceptions
        /// </summary>
        /// <param name="context"> The http context to handle </param>
        /// <returns></returns>
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await next(context);
            }
            catch (Exception ex)
            {
                var response = context.Response;
                response.ContentType = "application/json";

                var responseModel = new ApiResponse<string>
                {
                    StatusCode = (int)HttpStatusCode.InternalServerError,
                    Message = "Internal Server Error."
                };

                response.StatusCode = ex switch
                {
                    ApiException e => e.StatusCode,
                    KeyNotFoundException => (int)HttpStatusCode.NotFound,
                    _ => (int)HttpStatusCode.InternalServerError
                };

                var result = JsonSerializer.Serialize(responseModel);
                await response.WriteAsync(result);
            }
        }
    }
}
