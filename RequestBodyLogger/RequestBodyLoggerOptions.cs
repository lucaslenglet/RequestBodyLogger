namespace RequestBodyLogger;

public class RequestBodyLoggerOptions
{
    public static string SectionNameDefault => "RequestBodyLogger";

    public bool? Enabled { get; set; } = EnabledDefault;

    public bool? ApplyToAllEndpoints { get; set; } = ApplyToAllEndpointsDefault;

    public bool? LogEmptyBody { get; set; } = LogEmptyBodyDefault;
    
    public bool? LogEmptyBodyAsWarning { get; set; } = LogEmptyBodyAsWarningDefault;

    public RequestBodyLoggerMessagesOptions Messages { get; set; } = new();

    #region Defaults

    public static bool EnabledDefault => true;

    public static bool ApplyToAllEndpointsDefault => false;
    
    public static bool LogEmptyBodyDefault => false;

    public static bool LogEmptyBodyAsWarningDefault => false;

    #endregion
}

public class RequestBodyLoggerMessagesOptions
{
    public string? Normal { get; set; } = NormalDefault;

    public string? Empty { get; set; } = EmptyDefault;

    #region Defaults

    public static string NormalDefault => "Request received! {Body}";
    public static string EmptyDefault => "Request received with empty body!";

    #endregion
}