using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Rex.Application.Utilities;

namespace Trivo.Presentation.API.Filters;

public class ResultFilter(ILogger<ResultFilter> logger) : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var executedContext = await next();

        if (executedContext.Result is ObjectResult objectResult && objectResult.Value is Result result)
        {
            if (!result.IsSuccess)
            {
                int statusCode = GetStatusCodeFromError(result.Error);

                logger.LogWarning("Operation failed with code {Code} and message: {Message}", result.Error?.Code, result.Error?.Description);

                // Create an anonymous object with the error details
                var errorResponse = new
                {
                    code = result.Error?.Code,
                    description = result.Error?.Description
                };

                var objResult = new ObjectResult(errorResponse);

                objResult.StatusCode = statusCode;

                executedContext.Result = objResult;

                return;

            }

            // If the result is ResultT<T>, return its Value
            var type = result.GetType();
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(ResultT<>))
            {
                var value = type.GetProperty("Value")!.GetValue(result);
                executedContext.Result = new OkObjectResult(value);
            }
        }
    }

    private static int GetStatusCodeFromError(Error? error)
    {
        if (error != null && int.TryParse(error.Code, out var statusCode))
        {
            if (statusCode >= 100 && statusCode <= 599)
            {
                return statusCode;
            }
        }

        return StatusCodes.Status500InternalServerError;
    }

}