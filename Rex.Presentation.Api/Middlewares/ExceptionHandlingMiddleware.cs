using FluentValidation;
using Rex.Application.DTOs.Configs;
using Rex.Application.Utilities;


namespace Rex.Presentation.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var trackerId = Guid.NewGuid().ToString();
        context.Response.Headers["tracker-id"] = trackerId;

        try
        {
            await next(context);

            if (context.Response.StatusCode == StatusCodes.Status401Unauthorized)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ProblemDetails.Fail(
                    message: "Authentication required",
                    code: StatusCodes.Status401Unauthorized,
                    details: "You need to sign in to access this resource."
                ));
            }

            if (context.Response.StatusCode == StatusCodes.Status403Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsJsonAsync(ProblemDetails.Fail(
                    message: "Access denied",
                    code: StatusCodes.Status403Forbidden,
                    details: "You do not have permission to perform this action."
                ));
            }
        }
        catch (ValidationException ex)
        {
            logger.LogWarning(
                "Validation error: {Message} | TrackerId: {TrackerId} | Path: {Path}",
                ex.Message, trackerId, context.Request.Path
            );

            var errors = ex.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ProblemDetails.Fail(
                message: "Validation errors",
                code: StatusCodes.Status400BadRequest,
                details: "One or more validation errors occurred.",
                errors: errors
            ));
        }

        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception | TrackerId: {TrackerId} | Path: {Path}", trackerId, context.Request.Path);

            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(ProblemDetails.Fail(
                message: "Server error",
                code: StatusCodes.Status500InternalServerError,
                details: "An unexpected error occurred. Please try again later."
            ));
        }
    }
}
