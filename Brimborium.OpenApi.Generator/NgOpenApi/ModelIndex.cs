namespace Brimborium.OpenApi.NgOpenApi;

public class ModelIndex : GenType {
    public ModelIndex(List<Model> models, Options options) : base("models", n => n, options) {
        foreach (var model in models) {
            AddImport(model.Name);
        }
        UpdateImports();
    }

    protected override bool SkipImport() {
        return false;
    }

    protected override string InitPathToRoot() {
        return "./";
    }
}