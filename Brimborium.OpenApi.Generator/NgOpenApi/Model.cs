
namespace Brimborium.OpenApi.NgOpenApi;
public class Model : GenType {
    public bool IsSimple { get; set; }
    public bool IsEnum { get; set; }
    public bool IsObject { get; set; }

    public string SimpleType { get; set; }
    public List<EnumValue> EnumValues { get; set; }
    public string EnumArrayName { get; set; }
    public string EnumArrayFileName { get; set; }

    public string ElementType { get; set; }

    public List<Property> Properties { get; set; }
    public string AdditionalPropertiesType { get; set; }

    public Model(OpenAPIObject openApi, string name, SchemaObject schema, Options options) : base(name, unqualifiedName, options) {
        var description = schema.Description ?? "";
        TsComments = TsComments(description, 0, schema.Deprecated); // Assuming TsComments is a method in your C# class

        var type = schema.Type ?? "any";

        // Handle enums
        if ((schema.Enum ?? new List<object>()).Count > 0 && new List<string> { "string", "number", "integer" }.Contains(type)) {
            EnumArrayName = UpperCase(TypeName).Replace("\\s+", "_"); // Assuming UpperCase is a method in your C# class
            EnumArrayFileName = FileName(TypeName + "-array"); // Assuming FileName is a method in your C# class

            var names = schema.XEnumNames as List<string> ?? new List<string>();
            var descriptions = schema.XEnumDescriptions as List<string> ?? new List<string>();
            var values = schema.Enum ?? new List<object>();
            EnumValues = new List<EnumValue>();
            for (var i = 0; i < values.Count; i++) {
                var enumValue = new EnumValue(type, names[i], descriptions[i], values[i], options);
                EnumValues.Add(enumValue);
            }

            // When enumStyle is 'alias' it is handled as a simple type.
            IsEnum = options.EnumStyle != "alias";
        }

        var hasAllOf = schema.AllOf != null && schema.AllOf.Count > 0;
        var hasOneOf = schema.OneOf != null && schema.OneOf.Count > 0;
        IsObject = (type == "object" || schema.Properties != null) && !schema.Nullable && !hasAllOf && !hasOneOf;
        IsSimple = !IsObject && !IsEnum;

        if (IsObject) {
            // Object
            var propertiesByName = new Dictionary<string, Property>();
            CollectObject(schema, propertiesByName);
            var sortedNames = propertiesByName.Keys.ToList();
            sortedNames.Sort();
            Properties = sortedNames.Select(propName => propertiesByName[propName]).ToList();
        } else {
            // Simple / array / enum / union / intersection
            SimpleType = TsType(schema, options, openApi); // Assuming TsType is a method in your C# class
        }
        CollectImports(schema); // Assuming CollectImports is a method in your C# class
        UpdateImports(); // Assuming UpdateImports is a method in your C# class
    }

    protected override string InitPathToRoot() {
        if (Namespace != null) {
            // for each namespace level go one directory up
            // plus the "models" directory
            return string.Join("", Namespace.Split('/').Select(_ => "../")) + "../";
        }
        return "../";
    }

    protected override bool SkipImport(string name) {
        // Don't import own type
        return Name == name;
    }

    private void CollectObject(SchemaObject schema, Dictionary<string, Property> propertiesByName) {
        if (schema.Type == "object" || schema.Properties != null) {
            // An object definition
            var properties = schema.Properties ?? new Dictionary<string, object>();
            var required = schema.Required ?? new List<string>();
            var propNames = properties.Keys.ToList();

            // When there are additional properties, we need a union of all types for it.
            var propTypes = new HashSet<string>();
            Action<string> appendType = (type) => {
                if (type.StartsWith("null | ")) {
                    propTypes.Add("null");
                    propTypes.Add(type.Substring("null | ".Length));
                } else {
                    propTypes.Add(type);
                }
            };

            foreach (var propName in propNames) {
                var prop = new Property(this, propName, properties[propName], required.Contains(propName), Options, OpenApi);
                propertiesByName[propName] = prop;
                appendType(prop.Type);
                if (!prop.Required) {
                    propTypes.Add("undefined");
                }
            }

            if (schema.AdditionalProperties == true) {
                AdditionalPropertiesType = "any";
            } else if (schema.AdditionalProperties != null) {
                var propType = TsType(schema.AdditionalProperties, Options, OpenApi); // Assuming TsType is a method in your C# class
                appendType(propType);
                AdditionalPropertiesType = string.Join(" | ", propTypes.OrderBy(x => x));
            }
        }

        if (schema.AllOf != null) {
            foreach (var s in schema.AllOf) {
                CollectObject(s, propertiesByName);
            }
        }
    }
}
