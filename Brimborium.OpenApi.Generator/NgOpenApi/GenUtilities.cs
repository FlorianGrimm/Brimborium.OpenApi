using System.Text.RegularExpressions;

namespace Brimborium.OpenApi.NgOpenApi;
public static class GenUtilities {
    /// <summary>
    /// Returns the simple name, that is, the last part after '/'
    /// </summary>
    public static string SimpleName(string name) {
        int pos = name.LastIndexOf('/');
        return name.Substring(pos + 1);
    }

    /// <summary>
    /// Returns the unqualified model class name, that is, the last part after '.'
    /// </summary>
    public static string UnqualifiedName(string name, Options options) {
        int pos = name.LastIndexOf('.');
        return ModelClass(name.Substring(pos + 1), options);
    }

    /// <summary>
    /// Returns the qualified model class name, that is, the camelized namespace (if any) plus the unqualified name
    /// </summary>
    public static string QualifiedName(string name, Options options) {
        string ns = Namespace(name);
        string unq = UnqualifiedName(name, options);
        return ns != null ? TypeName(ns) + unq : unq;
    }

    /// <summary>
    /// Returns the filename where to write a model with the given name
    /// </summary>
    public static string ModelFile(string name, Options options) {
        string result = string.Empty;
        string ns = Namespace(name);
        if (ns != null) {
            result += $"/{ns}";
        }
        string file = UnqualifiedName(name, options);
        return result += $"/{FileName(file)}";
    }

    /// <summary>
    /// Returns the namespace path, that is, the part before the last '.' split by '/' instead of '.'.
    /// If there's no namespace, returns undefined.
    /// </summary>
    public static string Namespace(string name) {
        name = Regex.Replace(name, @"^\.+", "");
        name = Regex.Replace(name, @"\.$", "");
        int pos = name.LastIndexOf('.');
        return pos < 0 ? null : name.Substring(0, pos).Replace(".", "/");
    }

    private static readonly HashSet<string> ReservedKeywords = new HashSet<string>() { "abstract", "as", "base", "bool", "break", "byte", "case", "catch", "char", "checked", "class", "const", "continue", "decimal", "default", "delegate", "do", "double", "else", "enum", "event", "explicit", "extern", "false", "finally", "fixed", "float", "for", "foreach", "goto", "if", "implicit", "in", "int", "interface", "internal", "is", "lock", "long", "namespace", "new", "null", "object", "operator", "out", "override", "params", "private", "protected", "public", "readonly", "ref", "return", "sbyte", "sealed", "short", "sizeof", "stackalloc", "static", "string", "struct", "switch", "this", "throw", "true", "try", "typeof", "uint", "ulong", "unchecked", "unsafe", "ushort", "using", "virtual", "void", "volatile", "while" };

    /// <summary>
    /// If the given name is a JS reserved keyword, suffix it with a `$` character
    /// </summary>
    public static string EnsureNotReserved(string name) {
        return ReservedKeywords.Contains(name) ? name + "$" : name;
    }

    /// <summary>
    /// Returns the type (class) name for a given regular name
    /// </summary>
    public static string TypeName(string name, Options? options = null) {
        if (options?.DonotModifyTypeNames == true) {
            return UpperFirst(ToBasicChars(name));
        } else {
            return UpperFirst(MethodName(name));
        }
    }

    /// <summary>
    /// Returns the name of the enum constant for a given value
    /// </summary>
    /// <param name="value"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string EnumName(string value, Options options) {
        string name = ToBasicChars(value, true);
        if (options.EnumStyle == "ignorecase") {
            return name;
        } else if (options.EnumStyle == "upper") {
            name = name.ToUpper().Replace(" ", "_");
        } else {
            name = UpperFirst(CamelCase(name));
        }
        if (name[0] >= '0' && name[0] <= '9') {
            name = "$" + name;
        }
        return name;
    }

    /// <summary>
    /// Returns a suitable method name for the given name
    /// </summary>
    /// <param name="name">the raw name</param>
    /// <returns></returns>
    public static string MethodName(string name) {
        return CamelCase(ToBasicChars(name, true));
    }

    /// <summary>
    /// Returns the file name for a given type name
    /// </summary>
    /// <param name="text"></param>
    /// <returns></returns>
    public static string FileName(string text) {
        return KebabCase(ToBasicChars(text));
    }

    /// <summary>
    /// Converts a text to a basic, letters / numbers / underscore representation.
    /// When firstNonDigit is true, prepends the result with an uderscore if the first char is a digit.
    /// </summary>
    /// <param name="text"></param>
    /// <param name="firstNonDigit"></param>
    /// <returns></returns>
    public static string ToBasicChars(string text, bool firstNonDigit = false) {
        text = (text ?? string.Empty).Trim();
        text = Regex.Replace(text, @"[^\w$]+", "_");
        if (firstNonDigit && char.IsDigit(text[0])) {
            text = "_" + text;
        }
        return text;
    }

    /// <summary>
    /// Returns the TypeScript comments for the given schema description, in a given indentation level
    /// </summary>
    public static string TsComments(string description, int level, bool deprecated = false) {
        string indent = new string(' ', level * 2);
        if (string.IsNullOrEmpty(description)) {
            return indent + (deprecated ? "/** @deprecated */" : string.Empty);
        }
        var lines = description.Trim().Split('\n');
        StringBuilder result = new();
        result.Append($"\n{indent}/**\n");
        foreach (var line in lines) {
            var content = (string.IsNullOrEmpty(line) ? string.Empty : " " + line.Replace("*/", "* / "));
            result.Append($"{indent} *{content}\n");
        }
        if (deprecated) {
            result.Append($"{indent} *\n{indent} * @deprecated\n");
        }
        result.Append($"{indent} */\n{indent}");
        return result.ToString();
    }

    /// <summary>
    /// Applies the prefix and suffix to a model class name
    /// </summary>
    public static string ModelClass(string baseName, Options options) {
        return $"{options.ModelPrefix ?? string.Empty}{TypeName(baseName, options)}{options.ModelSuffix ?? string.Empty}";
    }

    /// <summary>
    /// Applies the prefix and suffix to a service class name
    /// </summary>
    /// <param name="baseName"></param>
    /// <param name="options"></param>
    /// <returns></returns>
    public static string ServiceClass(string baseName, Options options) {
        return $"{options.ServicePrefix ?? string.Empty}{TypeName(baseName, options)}{options.ServiceSuffix ?? "Service"}";
    }

    /// <summary>
    /// Escapes the name of a property / parameter if not valid JS identifier
    /// </summary>
    public static string EscapeId(string name) {
        if (Regex.IsMatch(name, @"^[a-zA-Z]\w*$")) {
            return name;
        } else {
            return $"'{name.Replace("'", "\\'")}'";
        }
    }

    /// <summary>
    /// Appends | null to the given type
    /// </summary>
    public static string MaybeAppendNull(string type, bool nullable) {
        if (type.Contains("null") || !nullable) {
            // The type itself already includes null
            return type;
        }
        return (type.Contains(" ") ? $"({type})" : type) + (nullable ? " | null" : "");
    }

    static string RawTsType(SchemaObject schema, Options options, OpenAPIObject openApi, Model container = null) {
        // An union of types
        var union = schema.OneOf ?? schema.AnyOf ?? new List<SchemaObject>();
        if (union.Count > 0) {
            if (union.Count > 1) {
                return $"({string.Join(" | ", union.Select(u => TsType(u, options, openApi, container)))})";
            } else {
                return string.Join(" | ", union.Select(u => TsType(u, options, openApi, container)));
            }
        }

        var type = schema.Type ?? "any";

        // An array
        if (type == "array" || schema.Items != null) {
            var items = schema.Items ?? new ItemsObject();
            var itemsType = TsType(items, options, openApi, container);
            return $"List<{itemsType}>";
        }

        // All the types
        var allOf = schema.AllOf ?? new List<SchemaObject>();
        var intersectionType = new List<string>();
        if (allOf.Count > 0) {
            intersectionType = allOf.Select(u => TsType(u, options, openApi, container)).ToList();
        }

        // An object
        if (type == "object" || schema.Properties != null) {
            var result = "{\n";
            var properties = schema.Properties ?? new Dictionary<string, SchemaObject>();
            var required = schema.Required;

            foreach (var baseSchema in allOf) {
                var discriminator = TryGetDiscriminator(baseSchema, schema, openApi);
                if (discriminator != null) {
                    result += $"'{discriminator.PropName}': '{discriminator.Value}';\n";
                }
            }

            foreach (var propName in properties.Keys) {
                var property = properties[propName];
                if (property == null) {
                    continue;
                }
                if (property.Description != null) {
                    result += TsComments(property.Description, 0, property.Deprecated);
                }
                result += $"'{propName}'";
                var propRequired = required != null && required.Contains(propName);
                if (!propRequired) {
                    result += "?";
                }
                var propertyType = TsType(property, options, openApi, container);
                result += $": {propertyType};\n";
            }
            if (schema.AdditionalProperties != null) {
                var additionalProperties = schema.AdditionalProperties == true ? new Dictionary<string, SchemaObject>() : schema.AdditionalProperties;
                result += $"[key: string]: {TsType(additionalProperties, options, openApi, container)};\n";
            }
            result += "}";
            intersectionType.Add(result);
        }

        if (intersectionType.Count > 0) {
            return string.Join(" & ", intersectionType);
        }

        // Inline enum
        var enumValues = schema.Enum ?? new List<object>();
        if (enumValues.Count > 0) {
            if (type == "number" || type == "integer" || type == "boolean") {
                return string.Join(" | ", enumValues);
            } else {
                return string.Join(" | ", enumValues.Select(v => $"'{Jsesc(v)}'"));
            }
        }

        // A Blob
        if (type == "string" && schema.Format == "binary") {
            return "Blob";
        }

        // A simple type (integer doesn't exist as type in JS, use number instead)
        return type == "integer" ? "number" : type;
    }

    /// <summary>
    /// Returns the TypeScript type for the given type and options
    /// </summary>
    /// <param name="schemaOrRef"></param>
    /// <param name="options"></param>
    /// <param name="openApi"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public static string TsType(SchemaOrRef schemaOrRef, Options options, OpenAPIObject openApi, Model container = null) {
        if (schemaOrRef == null) {
            // No schema
            return "any";
        }

        if (schemaOrRef.Ref != null) {
            // A reference
            var resolved = ResolveRef(openApi, schemaOrRef.Ref) as SchemaObject;
            var name = SimpleName(schemaOrRef.Ref);
            // When referencing the same container, use its type name
            return MaybeAppendNull((container != null && container.Name == name) ? container.TypeName : QualifiedName(name, options), resolved?.Nullable ?? false);
        }

        // Resolve the actual type (maybe nullable)
        var schema = schemaOrRef as SchemaObject;
        var type = RawTsType(schema, options, openApi, container);
        return MaybeAppendNull(type, schema?.Nullable ?? false);
    }

    /// <summary>
    /// Resolves a reference
    /// </summary>
    /// <param name="openApi"></param>
    /// <param name="refStr">The reference name, such as #/components/schemas/Name, or just Name</param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public static SchemaObject ResolveRef(OpenAPIObject openApi, string refStr) {
        if (!refStr.Contains('/')) {
            refStr = $"#/components/schemas/{refStr}";
        }
        object current = null;
        foreach (var part in refStr.Split('/')) {
            var trimmedPart = part.Trim();
            if (trimmedPart == "#" || trimmedPart == "") {
                current = openApi;
            } else if (current == null) {
                break;
            } else {
                current = ((Dictionary<string, object>)current)[trimmedPart];
            }
        }
        if (current == null || !(current is Dictionary<string, object>)) {
            throw new Exception($"Couldn't resolve reference {refStr}");
        }
        return current as SchemaObject;
    }

    /// <summary>
    /// Tries to get a discriminator info from a base schema and for a derived one.
    /// </summary>
    /// <param name="baseSchemaOrRef"></param>
    /// <param name="derivedSchema"></param>
    /// <param name="openApi"></param>
    /// <returns></returns>
    public static (string propName, string value)? TryGetDiscriminator(SchemaOrRef baseSchemaOrRef, SchemaObject derivedSchema, OpenAPIObject openApi) {
        var baseSchema = baseSchemaOrRef.Ref != null ? ResolveRef(openApi, baseSchemaOrRef.Ref) : baseSchemaOrRef as SchemaObject;
        var discriminatorProp = baseSchema?.Discriminator?.PropertyName;
        if (discriminatorProp != null) {
            var discriminatorValue = TryGetDiscriminatorValue(baseSchema, derivedSchema, openApi);
            if (discriminatorValue != null) {
                return (discriminatorProp, discriminatorValue);
            }
        }
        return null;
    }

    /// <summary>
    /// Tries to get a discriminator value from a base schema and for a derived one.
    /// </summary>
    /// <param name="baseSchema"></param>
    /// <param name="derivedSchema"></param>
    /// <param name="openApi"></param>
    /// <returns></returns>

    public static string TryGetDiscriminatorValue(SchemaObject baseSchema, SchemaObject derivedSchema, OpenAPIObject openApi) {
        var mapping = baseSchema.Discriminator?.Mapping;
        if (mapping != null) {
            var mappingIndex = Array.FindIndex(mapping.Values.ToArray(), refStr => ResolveRef(openApi, refStr) == derivedSchema);
            return mapping.Keys.ToArray()[mappingIndex] ?? null;
        }
        return null;
    }

#if false
    /**
 * Recursively deletes a directory
 */
    export function deleteDirRecursive(dir: string) {
        if (fs.existsSync(dir)) {
            fs.readdirSync(dir).forEach((file: any) => {
                const curPath = path.join(dir, file);
                if (fs.lstatSync(curPath).isDirectory()) { // recurse
                    deleteDirRecursive(curPath);
                } else { // delete file
                    fs.unlinkSync(curPath);
                }
            });
            fs.rmdirSync(dir);
        }
    }

/**
 * Synchronizes the files from the source to the target directory. Optionally remove stale files.
 */
export function syncDirs(srcDir: string, destDir: string, removeStale: boolean, logger: Logger): any {
  fs.ensureDirSync(destDir);
  const srcFiles = fs.readdirSync(srcDir);
  const destFiles = fs.readdirSync(destDir);
  for (const file of srcFiles) {
    const srcFile = path.join(srcDir, file);
    const destFile = path.join(destDir, file);
    if (fs.lstatSync(srcFile).isDirectory()) {
      // A directory: recursively sync
      syncDirs(srcFile, destFile, removeStale, logger);
    } else {
      // Read the content of both files and update if they differ
      const srcContent = fs.readFileSync(srcFile, { encoding: 'utf-8' });
      const destContent = fs.existsSync(destFile) ? fs.readFileSync(destFile, { encoding: 'utf-8' }) : null;
      if (srcContent !== destContent) {
        fs.writeFileSync(destFile, srcContent, { encoding: 'utf-8' });
        logger.debug('Wrote ' + destFile);
      }
    }
  }
  if (removeStale) {
    for (const file of destFiles) {
      const srcFile = path.join(srcDir, file);
      const destFile = path.join(destDir, file);
      if (!fs.existsSync(srcFile) && fs.lstatSync(destFile).isFile()) {
        fs.unlinkSync(destFile);
        logger.debug('Removed stale file ' + destFile);
      }
    }
  }
}
#endif
}
