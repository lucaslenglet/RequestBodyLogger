using Microsoft.Extensions.Options;

namespace RequestBodyLogger;

public class RequestBodyLoggerMiddleware : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var configuration = context.RequestServices.GetRequiredService<IOptionsSnapshot<RequestBodyLoggerOptions>>().Value!;

        if (configuration is not { Enabled: true })
        {
            await next(context);
            return;
        }

        var logAttribute = context
            .GetEndpoint()?
            .Metadata
            .OfType<LogBodyAttribute>()
            .FirstOrDefault();

        if (configuration is not { ApplyToAllEndpoints: true } && logAttribute is null)
        {
            await next(context);
            return;
        }

        if (logAttribute is not null)
        {
            if (!logAttribute.Enabled)
            {
                await next(context);
                return;
            }

            logAttribute.ApplyTo(configuration);
        }

        if (context.Request.ContentLength is not > 0)
        {
            if (configuration.LogEmptyBody ?? RequestBodyLoggerOptions.LogEmptyBodyDefault)
            {
                var logger = GetLogger(context);

                Action<string> logAction = (configuration.LogEmptyBodyAsWarning ?? false) switch
                {
                    true => s => logger.LogWarning(s),
                    false => s => logger.LogInformation(s)
                };

                logAction(configuration.Messages?.Empty ?? RequestBodyLoggerMessagesOptions.EmptyDefault);
            }

            await next(context);
            return;
        }

        var initialBody = context.Request.Body;

        try
        {
            using (var memStream = new MemoryStream())
            {
                await initialBody.CopyToAsync(memStream);

                memStream.Seek(0, SeekOrigin.Begin);

                using var sr = new StreamReader(memStream);
                var bodyContent = await sr.ReadToEndAsync();

                if (!string.IsNullOrWhiteSpace(bodyContent))
                {
                    var logger = GetLogger(context);
                    logger.LogInformation(
                        configuration.Messages.Normal ?? RequestBodyLoggerMessagesOptions.NormalDefault, bodyContent);
                }

                memStream.Seek(0, SeekOrigin.Begin);
                context.Request.Body = memStream;

                await next(context);
            }
        }
        finally
        {
            context.Request.Body = initialBody;
        }
    }

    private static ILogger<RequestBodyLoggerMiddleware> GetLogger(HttpContext context)
    {
        return context.RequestServices.GetRequiredService<ILogger<RequestBodyLoggerMiddleware>>();
    }
}