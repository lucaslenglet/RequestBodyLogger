namespace RequestBodyLogger;

public class RequestBodyLoggerOptions
{
    public static string SectionNameDefault => "RequestBodyLogger";

    public bool? Allow { get; set; } = AllowDefault;

    public static bool AllowDefault => true;

    public string? MessageTemplate { get; set; } = MessageTemplateDefault;
    
    public static string MessageTemplateDefault => "Body : {Body}";
}