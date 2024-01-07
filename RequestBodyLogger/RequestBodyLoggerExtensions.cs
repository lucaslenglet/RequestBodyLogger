using Microsoft.Extensions.Options;

namespace RequestBodyLogger;

public static class RequestBodyLoggerExtensions
{
    public static IServiceCollection AddRequestBodyLogger(this IServiceCollection serviceCollection)
    {
        return serviceCollection.AddRequestBodyLogger(RequestBodyLoggerOptions.SectionNameDefault, null);
    }
    
    public static IServiceCollection AddRequestBodyLogger(this IServiceCollection serviceCollection, string sectionName)
    {
        return serviceCollection.AddRequestBodyLogger(sectionName, null);
    }
    
    public static IServiceCollection AddRequestBodyLogger(this IServiceCollection serviceCollection, Action<RequestBodyLoggerOptions> configure)
    {
        return serviceCollection.AddRequestBodyLogger(null, configure);
    }
    
    public static IApplicationBuilder UseRequestBodyLogger(this IApplicationBuilder app)
    {
        app.Use(async (context, next) =>
        {
            var configuration = context.RequestServices.GetService<IOptionsSnapshot<RequestBodyLoggerOptions>>()?.Value;

            if (configuration is { Allow: true })
            {
                context.Request.EnableBuffering();
            }
    
            await next();
        });
        
        return app;
    }
    
    private static IServiceCollection AddRequestBodyLogger(
        this IServiceCollection serviceCollection, 
        string? sectionName = null,
        Action<RequestBodyLoggerOptions>? configure = default)
    {
        var optionsBuilder = serviceCollection.AddOptions<RequestBodyLoggerOptions>();

        if (sectionName is not null)
        {
            optionsBuilder.BindConfiguration(sectionName);
        }

        if (configure is not null)
        {
            optionsBuilder.Configure(configure);
        }

        return serviceCollection;
    }
}