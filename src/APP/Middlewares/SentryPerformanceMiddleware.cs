using Microsoft.AspNetCore.Http;

namespace APP.Middlewares
{
    public class SentryPerformanceMiddleware(RequestDelegate next, IHub sentryHub)
    {
        public async Task InvokeAsync(HttpContext context)
        {
            // Start a transaction span for the incoming HTTP request
            var transaction = sentryHub.StartTransaction(context.Request.Path, context.Request.Method);

            sentryHub.ConfigureScope(scope => scope.Transaction = transaction);

            try
            {
                await next(context); // Process the next middleware in the pipeline
                transaction.Finish(SpanStatus.Ok);
            }
            catch (Exception ex)
            {
                transaction.Finish(ex); 
                throw; 
            }
        }
    }
}