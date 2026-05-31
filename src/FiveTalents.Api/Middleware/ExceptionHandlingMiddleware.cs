using System.Text.Json;

using FiveTalents.Application.Common.Exceptions;

namespace FiveTalents.Api.Middleware;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, title, errors) = exception switch
        {
            ValidationException ve => (StatusCodes.Status400BadRequest, "Validation Error", (object)ve.Errors),
            NotFoundException nfe => (StatusCodes.Status404NotFound, "Not Found", (object)new { message = nfe.Message }),
            ForbiddenAccessException => (StatusCodes.Status403Forbidden, "Forbidden", (object)new { message = "Access denied" }),
            InvalidOperationException ioe => (StatusCodes.Status400BadRequest, "Bad Request", (object)new { message = ioe.Message }),
            _ => (StatusCodes.Status500InternalServerError, "Internal Server Error", (object)new { message = "An unexpected error occurred" })
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = statusCode;

        await context.Response.WriteAsync(JsonSerializer.Serialize(new { title, errors }, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase }));
    }
}
