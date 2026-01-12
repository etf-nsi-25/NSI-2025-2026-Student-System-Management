using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Application
{
    public sealed class GlobalExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<GlobalExceptionHandlingMiddleware> _logger;
        private readonly IHostEnvironment _env;

        public GlobalExceptionHandlingMiddleware(
            RequestDelegate next,
            ILogger<GlobalExceptionHandlingMiddleware> logger,
            IHostEnvironment env)
        {
            _next = next;
            _logger = logger;
            _env = env;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (OperationCanceledException ex)
            {
                _logger.LogWarning(ex, "Request cancelled/timeout.");
                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError,
                    "The request was cancelled or the operation timed out.");
            }
            catch (NpgsqlException ex)
            {
                _logger.LogError(ex, "Database connection error.");
                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError,
                    "Database connection error.");
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogWarning(ex, "Concurrency error.");
                await WriteJsonAsync(context, StatusCodes.Status409Conflict,
                    "A concurrency error occurred. Please retry.");
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database update error.");
                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError,
                    "Database update error.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception.");

                // u dev možeš vratiti više detalja, u prod generic poruku
                var msg = _env.IsDevelopment() ? ex.Message : "Internal Server Error";

                await WriteJsonAsync(context, StatusCodes.Status500InternalServerError, msg);
            }
        }

        private static async Task WriteJsonAsync(HttpContext context, int statusCode, string message)
        {
            if (context.Response.HasStarted) return;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/json";

            await context.Response.WriteAsJsonAsync(new { error = message });
        }
    }
}
