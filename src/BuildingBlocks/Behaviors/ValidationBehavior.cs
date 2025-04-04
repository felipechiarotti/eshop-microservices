using FluentValidation;
using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;
public class ValidationBehavior<TRequest, TResponse>(IEnumerable<IValidator<TRequest>> validators, ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var context = new ValidationContext<TRequest>(request);
        var validationResults = await Task.WhenAll(validators.Select(v => v.ValidateAsync(context, cancellationToken)));

        var failedValidations = validationResults
            .Where(r => r.Errors.Any())
            .SelectMany(r => r.Errors)
            .ToList();

        if (failedValidations.Any())
        {
            var validationException = new ValidationException(failedValidations);
            //logger.LogWarning("{RequestType}: {ValidationErrorMessage}", typeof(TRequest).Name, validationException.Message);
            throw validationException;
        }

        var response = await next();
        return response;
    }
}