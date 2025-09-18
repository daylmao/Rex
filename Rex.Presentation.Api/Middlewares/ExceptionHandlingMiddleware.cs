using FluentValidation;
using Microsoft.AspNetCore.Mvc;

namespace Rex.Presentation.Api.Middlewares;

public class ExceptionHandlingMiddleware(
    RequestDelegate next, 
    ILogger<ExceptionHandlingMiddleware> logger)
{
    public async Task InvokeAsync(HttpContext context)
    {
        var trackerId = Guid.NewGuid().ToString();
        context.Response.Headers.Append("x-tracker-id", trackerId);

        try
        {
            await next(context);
        }
        catch (ValidationException e)
        {
            logger.LogError(e, e.Message);

            var problemDetails = new ProblemDetails
            {
                Status = StatusCodes.Status400BadRequest,
                Title = "Validation Errors",
                Detail = "You have one or more validation errors",
                Type = "ValidationFailure",
                Instance = context.Request.Path
            };

            problemDetails.Extensions["errors"] = e.Errors
                .GroupBy(e => e.PropertyName)
                .ToDictionary(
                    g => g.Key,
                    g => g.Select(e => e.ErrorMessage).ToArray()
                );

            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);

        }
        catch (Exception ex)
        {
            logger.LogError(ex, ex.Message);

            var problemDetails = new ProblemDetails()
            {
                Title = "Internal Server Error",
                Status = StatusCodes.Status500InternalServerError,
                Detail = ex.Message,
                Instance = context.Request.Path
            };
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            await context.Response.WriteAsJsonAsync(problemDetails);
        }
    }
}