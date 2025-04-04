using MediatR;
using Microsoft.Extensions.Logging;

namespace BuildingBlocks.Behaviors;
public class LoggingBehavior<TRequest, TResponse>(ILogger<TRequest> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull, IRequest<TResponse>
    where TResponse : notnull
{

    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        logger.LogInformation("Request {RequestType} being sent: {@Request}", typeof(TRequest).Name, request);
        var response = await next();
        logger.LogInformation("Response {ResponseType} for the {RequestType}: {@Response}", typeof(TResponse).Name, typeof(TRequest).Name, response);
        return response;
    }
}