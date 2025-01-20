using FluentValidation;
using Newtonsoft.Json;
using System.Net;

namespace ClinicalTrials.API.Common
{
    public class ErrorHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ErrorHandlingMiddleware> _logger;
        public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }
        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                await HandleExceptionAsync(context, ex);
            }
        }
        private static Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            HttpStatusCode code;
            string result;

            switch (exception)
            {
                case ValidationException _:
                    code = HttpStatusCode.BadRequest;
                    result = JsonConvert.SerializeObject(new { code, error = "Provided file is not valid." });
                    break;
                case KeyNotFoundException _:
                    code = HttpStatusCode.NotFound;
                    result = JsonConvert.SerializeObject(new { code, error = "Clinical trial not found." });
                    break;
                default:
                    code = HttpStatusCode.InternalServerError;
                    result = JsonConvert.SerializeObject(new { code, error = exception.Message });
                    break;
            }

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)code;
            return context.Response.WriteAsync(result);
        }
    }
}
