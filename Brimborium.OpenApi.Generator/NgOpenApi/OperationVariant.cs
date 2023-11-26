using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brimborium.OpenApi.NgOpenApi;
public class OperationVariant : GenType, IImportable {
    public string ResponseMethodName { get; set; }
    public string ResultType { get; set; }
    public string ResponseType { get; set; }
    public string Accept { get; set; }
    public bool IsVoid { get; set; }
    public bool IsNumber { get; set; }
    public bool IsBoolean { get; set; }
    public bool IsOther { get; set; }
    public string ResponseMethodTsComments { get; set; }
    public string BodyMethodTsComments { get; set; }

    public string ParamsType { get; set; }
    public IImportable ParamsImport { get; set; }

    public string ImportName { get; set; }
    public string ImportPath { get; set; }
    public string ImportFile { get; set; }

    public Operation Operation { get; set; }
    public string MethodName { get; set; }
    public Content RequestBody { get; set; }
    public Content SuccessResponse { get; set; }
    public Options Options { get; set; }

    public OperationVariant(Operation operation, string methodName, Content requestBody, Content successResponse, Options options)
        : base(methodName, n => n, options) {
        Operation = operation;
        MethodName = methodName;
        RequestBody = requestBody;
        SuccessResponse = successResponse;
        Options = options;

        ResponseMethodName = $"{methodName}$Response";
        if (successResponse != null) {
            ResultType = successResponse.Type;
            ResponseType = InferResponseType(successResponse, operation, options);
            Accept = successResponse.MediaType;
        } else {
            ResultType = "void";
            ResponseType = "text";
            Accept = "*/*";
        }
        IsVoid = ResultType == "void";
        IsNumber = ResultType == "number";
        IsBoolean = ResultType == "boolean";
        IsOther = !IsVoid && !IsNumber && !IsBoolean;
        ResponseMethodTsComments = TsComments(ResponseMethodDescription(), 1, operation.Deprecated);
        BodyMethodTsComments = TsComments(BodyMethodDescription(), 1, operation.Deprecated);

        ImportPath = "fn/" + FileName(operation.Tags[0] ?? options.DefaultTag ?? "operations");
        ImportName = EnsureNotReserved(methodName);
        ImportFile = FileName(methodName);

        ParamsType = $"{UpperFirst(methodName)}$Params";
        ParamsImport = new Importable {
            ImportName = ParamsType,
            ImportFile = ImportFile,
            ImportPath = ImportPath
        };

        // Collect parameter imports
        foreach (var parameter in operation.Parameters) {
            CollectImports(parameter.Spec.Schema, false, true);
        }
        // Collect the request body imports
        CollectImports(requestBody?.Spec?.Schema);
        // Collect the response imports
        CollectImports(successResponse?.Spec?.Schema);

        // Finally, update the imports
        UpdateImports();
    }

    private string InferResponseType(Content successResponse, Operation operation, Options options) {
        var customizedResponseTypeByPath = options.CustomizedResponseType[operation.Path];
        if (customizedResponseTypeByPath != null) {
            return customizedResponseTypeByPath.ToUse;
        }

        // When the schema is in binary format, return 'blob'
        var schemaOrRef = successResponse.Spec?.Schema ?? new SchemaObject { Type = "string" };
        if (schemaOrRef.Ref != null) {
            schemaOrRef = ResolveRef(operation.OpenApi, schemaOrRef.Ref); // Assuming ResolveRef is a method in your C# class
        }
        var schema = schemaOrRef;
        if (schema.Format == "binary") {
            return "blob";
        }

        var mediaType = successResponse.MediaType.ToLower();
        if (mediaType.Contains("/json") || mediaType.Contains("+json")) {
            return "json";
        } else if (mediaType.StartsWith("text/")) {
            return "text";
        } else {
            return "blob";
        }
    }

    private string ResponseMethodDescription() {
        return $"{DescriptionPrefix()}This method provides access to the full `HttpResponse`, allowing access to response headers.\nTo access only the response body, use `{MethodName}()` instead.{DescriptionSuffix()}";
    }

    private string BodyMethodDescription() {
        return $"{DescriptionPrefix()}This method provides access only to the response body.\nTo access the full response (for headers, for example), `{ResponseMethodName}()` instead.{DescriptionSuffix()}";
    }

    private string DescriptionPrefix() {
        var description = (Operation.Spec.Description ?? "").Trim();
        var summary = Operation.Spec.Summary;
        if (summary != null) {
            if (!summary.EndsWith(".")) {
                summary += ".";
            }
            description = summary + "\n\n" + description;
        }
        if (description != "") {
            description += "\n\n";
        }
        return description;
    }

    private string DescriptionSuffix() {
        var sends = RequestBody != null ? $"sends `{RequestBody.MediaType}` and " : "";
        var handles = RequestBody != null
            ? $"handles request body of type `{RequestBody.MediaType}`"
            : "doesn't expect any request body";
        return $"\n\nThis method {sends}{handles}.";
    }

    protected override bool SkipImport() {
        return false;
    }

    protected override string InitPathToRoot() {
        return string.Join("/", ImportPath.Split('/').Select(_ => "..")) + "/";
    }    
}