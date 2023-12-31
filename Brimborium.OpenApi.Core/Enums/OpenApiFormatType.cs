namespace Brimborium.OpenApi.Enums; 
/// <summary>
/// This specifies the OpenAPI format.
/// </summary>
public enum OpenApiFormatType {
    /// <summary>
    /// Identifies the JSON format.
    /// </summary>
    [Display("json")]
    Json = 0,

    /// <summary>
    /// Identifies the YAML format.
    /// </summary>
    [Display("yaml")]
    Yaml = 1
}
