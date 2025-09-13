using FluentValidation;
using MediatR;

namespace Rex.Application.Behavior;

public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators):
    IPipelineBehavior<TRequest, TResponse>
    where TRequest : class
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(request);

        if (validators.Any())
        {
            var context = new ValidationContext<TRequest>(request);
            var validationResults = await Task.WhenAll(validators
                .Select(v => v.ValidateAsync(context, cancellationToken)))
                .ConfigureAwait(false);

            var failures = validationResults
                .Where(v => v.Errors.Count > 0)
                .SelectMany(s => s.Errors)
                .ToList();
            
            if(failures.Count >0)
                throw new ValidationException(failures);
        }
        
        return await next().ConfigureAwait(false);
    }
}