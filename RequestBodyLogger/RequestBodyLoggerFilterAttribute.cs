using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Options;

namespace RequestBodyLogger;

[AttributeUsage(AttributeTargets.Method)]
public class RequestBodyLoggerFilterAttribute : Attribute, IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var configuration = context.HttpContext.RequestServices.GetService<IOptionsSnapshot<RequestBodyLoggerOptions>>()?.Value;

        if (configuration is { Allow: true })
        {
            await next();
            return;
        }

        var request = context.HttpContext.Request;

        if (request.ContentLength is not > 0 || !request.Body.CanSeek)
        {
            await next();
            return;
        }

        using (var sr = new StreamReader(request.Body))
        {
            request.Body.Seek(0, SeekOrigin.Begin);

            var bodyContent = await sr.ReadToEndAsync();

            if (!string.IsNullOrWhiteSpace(bodyContent))
            {
                var logger = context.HttpContext.RequestServices.GetRequiredService<ILogger<RequestBodyLoggerFilterAttribute>>();
                logger.LogInformation(configuration?.MessageTemplate ?? RequestBodyLoggerOptions.MessageTemplateDefault, bodyContent);
            }

            request.Body.Seek(0, SeekOrigin.Begin);
        }

        await next();
    }
}