namespace Brimborium.OpenApi.NgOpenApi;
public class Options {
    public string? Input { get; set; }
    public string? Output { get; set; }
    public string? DefaultTag { get; set; }
    public int? FetchTimeout { get; set; }
    public List<string> IncludeTags { get; set; } = new();
    public List<string> ExcludeTags { get; set; } = new();
    public bool? IgnoreUnusedModels { get; set; }
    public bool? RemoveStaleFiles { get; set; }
    public object? ModelIndex { get; set; } // string or bool
    public object? ServiceIndex { get; set; } // string or bool
    public bool? IndexFile { get; set; }
    public bool? Services { get; set; }
    public string? ServicePrefix { get; set; }
    public string? ServiceSuffix { get; set; }
    public string? ModelPrefix { get; set; }
    public string? ModelSuffix { get; set; }
    public bool? ApiModule { get; set; }
    public string? Configuration { get; set; }
    public string? BaseService { get; set; }
    public string? ApiService { get; set; }
    public string? RequestBuilder { get; set; }
    public string? Response { get; set; }
    public object? Module { get; set; } // string or bool
    public string? EnumStyle { get; set; } // 'alias' | 'upper' | 'pascal' | 'ignorecase'
    public bool? EnumArray { get; set; }
    public string? EndOfLineStyle { get; set; } // 'crlf' | 'lf' | 'cr' | 'auto'
    public string? Templates { get; set; }
    public List<string> ExcludeParameters { get; set; } = new();
    public bool? SkipJsonSuffix { get; set; }
    public Dictionary<string, CustomizedResponseType> CustomizedResponseType { get; set; } = new();
    public bool? UseTempDir { get; set; }
    public bool? Silent { get; set; }
    public bool? DonotModifyTypeNames { get; set; }
}

public class CustomizedResponseType {
    public string? ToUse { get; set; } // 'arraybuffer' | 'blob' | 'json' | 'document'
}