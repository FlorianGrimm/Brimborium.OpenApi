namespace Brimborium.OpenApi.NgOpenApi;

// Either a request body or response content
public class Content {
    public string MediaType { get; set; }
    public MediaTypeObject Spec { get; set; }
    public Options Options { get; set; }
    public OpenAPIObject OpenApi { get; set; }
    public string Type { get; set; }

    public Content(string mediaType, MediaTypeObject spec, Options options, OpenAPIObject openApi) {
        MediaType = mediaType;
        Spec = spec;
        Options = options;
        OpenApi = openApi;
        Type = TsType(spec.Schema, options, openApi); // Assuming TsType is a method in your C# class
    }
}

public class EnumValue {
    public string Name { get; set; }
    public string Value { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
    public Options Options { get; set; }

    public EnumValue(string type, string name, string description, object value, Options options) {
        Type = type;
        Options = options;
        Value = value.ToString();
        Name = name ?? EnumName(Value, options); // Assuming EnumName is a method in your C# class
        Description = description ?? Name;
        if (string.IsNullOrEmpty(Name)) {
            Name = "_";
        }
        if (string.IsNullOrEmpty(Description)) {
            Description = Name;
        }
        if (Type == "string") {
            Value = $"'{Jsesc(Value)}'"; // Assuming Jsesc is a method in your C# class
        }
    }
}