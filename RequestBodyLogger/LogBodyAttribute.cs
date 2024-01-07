using Microsoft.AspNetCore.Mvc.Filters;

namespace RequestBodyLogger;

[AttributeUsage(AttributeTargets.Method)]
public class LogBodyAttribute : Attribute, IAsyncActionFilter
{
    public EmptyBodyAction EmptyBody { get; set; } = EmptyBodyAction.NotSet;
    public string? NormalMessageTemplate { get; set; }
    public string? EmptyMessageTemplate { get; set; }
    public bool Enabled { get; set; } = true;
    
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        await next();
    }

    public void ApplyTo(RequestBodyLoggerOptions options)
    {
        if (EmptyBody is not EmptyBodyAction.NotSet)
        {
            options.LogEmptyBody = EmptyBody switch
            {
                EmptyBodyAction.LogInfo or EmptyBodyAction.LogWarning => true,
                EmptyBodyAction.NoLog => false,
                _ => options.LogEmptyBody
            };

            options.LogEmptyBodyAsWarning = EmptyBody switch
            {
                EmptyBodyAction.LogWarning => true,
                EmptyBodyAction.LogInfo or EmptyBodyAction.NoLog => false,
                _ => options.LogEmptyBodyAsWarning
            };
        }

        if (!string.IsNullOrWhiteSpace(NormalMessageTemplate))
        {
            options.Messages.Normal = NormalMessageTemplate;
        }
        
        if (!string.IsNullOrWhiteSpace(EmptyMessageTemplate))
        {
            options.Messages.Empty = EmptyMessageTemplate;
        }
    }
}