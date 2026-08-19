using Microsoft.AspNetCore.Diagnostics;
using Kipas.Personel.API.Helpers;

namespace Kipas.Personel.API.Middleware
{
    public sealed class GlobalExceptionHandler : IExceptionHandler
    {
        private readonly ILogger<GlobalExceptionHandler> _logger;

        public GlobalExceptionHandler(
            ILogger<GlobalExceptionHandler> logger)
        {
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Beklenmeyen bir hata oluştu. TraceId: {TraceId}",
                httpContext.TraceIdentifier);

            var response = new ApiResponse<object?>
            {
                Success = false,
                Message = "İşlem sırasında beklenmeyen bir hata oluştu.",
                TraceId = httpContext.TraceIdentifier
            };

            httpContext.Response.StatusCode =
                StatusCodes.Status500InternalServerError;

            httpContext.Response.ContentType = "application/json";

            await httpContext.Response.WriteAsJsonAsync(
                response,
                cancellationToken);

            return true;
        }
    }
}