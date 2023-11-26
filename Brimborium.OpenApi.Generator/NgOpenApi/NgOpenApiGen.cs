

namespace Brimborium.OpenApi.NgOpenApi;

public class NgOpenApiGen {
    public Dictionary<string, Model> Models { get; set; }
    public Dictionary<string, Service> Services { get; set; }
    public Dictionary<string, Operation> Operations { get; set; }
    public string OutDir { get; set; }
    public ILogger Logger { get; set; }
    public string TempDir { get; set; }

    public NgOpenApiGen(OpenAPIObject openApi, Options options) {
        // Constructor logic
    }

    public void InitTempDir() {
        // Method logic
    }

    public void Generate() {
        // Method logic
    }

    private void Write(string template, object model, string baseName, string subDir) {
        // Method logic
    }

    private void InitHandlebars() {
        // Method logic
    }

    private void ReadTemplates() {
        // Method logic
    }

    private void ReadModels() {
        // Method logic
    }

    private void ReadServices() {
        // Method logic
    }

    private void IgnoreUnusedModels() {
        // Method logic
    }

    private void CollectDependencies(string name, HashSet<string> usedNames) {
        // Method logic
    }

    private List<string> AllReferencedNames(SchemaObject schema) {
        // Method logic
    }

    private string SetEndOfLine(string text) {
        // Method logic
    }
}

public async void RunNgOpenApiGen() {
    var options = ParseOptions();
    var refParser = new RefParser();
    var input = options.Input;

    try {
        var openApi = await refParser.Bundle(input, new BundleOptions {
            Dereference = new DereferenceOptions { Circular = false },
            Resolve = new ResolveOptions { Http = new HttpOptions { Timeout = options.FetchTimeout ?? 20000 } }
        });

        var gen = new NgOpenApiGen(openApi, options);
        gen.Generate();
    } catch (Exception ex) {
        Console.WriteLine($"Error on API generation from {input}: {ex.Message}");
        Environment.Exit(1);
    }
}