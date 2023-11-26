namespace Brimborium.OpenApi.NgOpenApi;

public class Globals {
    public string ConfigurationClass { get; set; }
    public string ConfigurationFile { get; set; }
    public string ConfigurationParams { get; set; }
    public string BaseServiceClass { get; set; }
    public string BaseServiceFile { get; set; }
    public string? ApiServiceClass { get; set; }
    public string? ApiServiceFile { get; set; }
    public string RequestBuilderClass { get; set; }
    public string RequestBuilderFile { get; set; }
    public string ResponseClass { get; set; }
    public string ResponseFile { get; set; }
    public string? ModuleClass { get; set; }
    public string? ModuleFile { get; set; }
    public string? ModelIndexFile { get; set; }
    public string? ServiceIndexFile { get; set; }
    public string? RootUrl { get; set; }

    public Globals(Options options) {
        ConfigurationClass = options.Configuration ?? "ApiConfiguration";
        ConfigurationFile = FileName(ConfigurationClass); // Assuming FileName is a method in your C# class
        ConfigurationParams = $"{ConfigurationClass}Params";
        BaseServiceClass = options.BaseService ?? "BaseService";
        BaseServiceFile = FileName(BaseServiceClass);
        ApiServiceClass = options.ApiService ?? "";
        if (string.IsNullOrEmpty(ApiServiceClass)) {
            ApiServiceClass = null;
        } else {
            // Angular's best practices demands xxx.service.ts, not xxx-service.ts
            ApiServiceFile = FileName(ApiServiceClass).Replace("-service", ".service");
        }
        RequestBuilderClass = options.RequestBuilder ?? "RequestBuilder";
        RequestBuilderFile = FileName(RequestBuilderClass);
        ResponseClass = options.Response ?? "StrictHttpResponse";
        ResponseFile = FileName(ResponseClass);
        if (options.Module != false && !string.IsNullOrEmpty(options.Module)) {
            ModuleClass = options.Module == true || options.Module == null ? "ApiModule" : options.Module;
            // Angular's best practices demands xxx.module.ts, not xxx-module.ts
            ModuleFile = FileName(ModuleClass ?? "").Replace("-module", ".module");
        }
        if (options.ServiceIndex != false && !string.IsNullOrEmpty(options.ServiceIndex)) {
            ServiceIndexFile = options.ServiceIndex == true || options.ServiceIndex == null ? "services" : options.ServiceIndex;
        }
        if (options.ModelIndex != false && !string.IsNullOrEmpty(options.ModelIndex)) {
            ModelIndexFile = options.ModelIndex == true || options.ModelIndex == null ? "models" : options.ModelIndex;
        }
    }
}