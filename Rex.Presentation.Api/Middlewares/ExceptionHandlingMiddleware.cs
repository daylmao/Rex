using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

namespace Rex.Presentation.Api.Middlewares;

public class ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var trackerId = Guid.NewGuid().ToString();
        context.Response.Headers.Append("x-tracker-id", trackerId);

        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await HandleErrorAsync(context, ex, trackerId);
        }
    }

    private async Task HandleErrorAsync(HttpContext context, Exception ex, string trackerId)
    {
        if (context.Response.HasStarted)
        {
            logger.LogWarning("Cannot handle error - response already started. Tracker: {TrackerId}", trackerId);
            return;
        }

        var (statusCode, title) = GetErrorDetails(ex);

        logger.LogError(ex, "Error {StatusCode}: {Message} | Tracker: {TrackerId}", statusCode, ex.Message, trackerId);

        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = ex.Message,
            Instance = context.Request.Path,
            Extensions = { ["trackerId"] = trackerId }
        };

        if (ex is ValidationException validationEx)
        {
            problemDetails.Extensions["errors"] = validationEx.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );
        }

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsJsonAsync(problemDetails);
    }

    private static (int statusCode, string title) GetErrorDetails(Exception ex)
    {
        return ex switch
        {
            ValidationException => (400, "Validation Error"),
            SecurityTokenExpiredException => (401, "Token Expired"),
            SecurityTokenException => (401, "Invalid Token"),
            UnauthorizedAccessException => (403, "Access Denied"),
            _ => (500, "Internal Server Error")
        };
    }
}