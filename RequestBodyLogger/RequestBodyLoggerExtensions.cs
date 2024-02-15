using Microsoft.Extensions.DependencyInjection.Extensions;

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
        return app.UseMiddleware<RequestBodyLoggerMiddleware>();
    }
    
    private static IServiceCollection AddRequestBodyLogger(
        this IServiceCollection serviceCollection, 
        string? sectionName = default,
        Action<RequestBodyLoggerOptions>? configure = default)
    {
        if (sectionName is null && configure is null)
        {
            throw new ArgumentNullException();
        }
        
        serviceCollection.TryAddTransient<RequestBodyLoggerMiddleware>();
        
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