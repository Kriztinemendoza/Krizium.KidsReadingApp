using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;

namespace Krizium.KidsReadingApp.Api.Middleware
{
    public class ExceptionHandlingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddleware> _logger;

        public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception");
                await HandleExceptionAsync(context, ex);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
        {
            var response = new ErrorResponse
            {
                StatusCode = GetStatusCodeFromException(exception),
                Message = GetUserFriendlyMessage(exception),
                TraceId = context.TraceIdentifier
            };

            context.Response.ContentType = "application/json";
            context.Response.StatusCode = (int)response.StatusCode;

            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            var json = JsonSerializer.Serialize(response, options);
            await context.Response.WriteAsync(json);
        }

        private static HttpStatusCode GetStatusCodeFromException(Exception exception)
        {
            // Map exception types to HTTP status codes
            return exception switch
            {
                ArgumentException _ => HttpStatusCode.BadRequest,
                KeyNotFoundException _ => HttpStatusCode.NotFound,
                UnauthorizedAccessException _ => HttpStatusCode.Unauthorized,
                NotImplementedException _ => HttpStatusCode.NotImplemented,
                _ => HttpStatusCode.InternalServerError
            };
        }

        private static string GetUserFriendlyMessage(Exception exception)
        {
            // Return user-friendly messages for known exception types
            return exception switch
            {
                ArgumentException _ => exception.Message,
                KeyNotFoundException _ => exception.Message,
                UnauthorizedAccessException _ => "You are not authorized to perform this action",
                NotImplementedException _ => "This feature is not yet implemented",
                _ => "An unexpected error occurred. Please try again later."
            };
        }
    }

    public class ErrorResponse
    {
        public HttpStatusCode StatusCode { get; set; }
        public string Message { get; set; } = string.Empty;
        public string TraceId { get; set; } = string.Empty;
    }
}
