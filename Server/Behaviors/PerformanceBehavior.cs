using System.Diagnostics;

namespace Server.Behaviors;

public class PerformanceBehavior<TRequest, TResponse>
    (ILogger<PerformanceBehavior<TRequest, TResponse>> logger)
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
    where TResponse : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var timer = new Stopwatch();
        timer.Start();
        var response = await next();
        timer.Stop();
        var timeTaken = timer.Elapsed;

        logger.LogInformation("[PERFORMANCE] {Request} took {TimeTaken}", typeof(TRequest).Name, timeTaken);

        return response;
    }
}