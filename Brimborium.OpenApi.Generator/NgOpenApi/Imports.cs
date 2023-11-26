namespace Brimborium.OpenApi.NgOpenApi;

public class Imports {
    private Dictionary<string, Import> _imports = new Dictionary<string, Import>();
    private Options options;

    public Imports(Options options) {
        this.options = options;
    }

    // Adds an import
    public void Add(IImportable param) {
        Import imp;
        if (param is string) {
            // A model
            imp = new Import(param, UnqualifiedName(param, this.options), QualifiedName(param, this.options), "models/", ModelFile(param, this.options));
        } else {
            // An Importable
            imp = new Import(param.ImportName, param.ImportTypeName ?? param.ImportName, param.ImportQualifiedName ?? param.ImportName, $"{param.ImportPath}", param.ImportFile);
        }
        this._imports[imp.Name] = imp;
    }

    public List<Import> ToArray() {
        var array = this._imports.Values.ToList();
        array.Sort((a, b) => a.ImportName.CompareTo(b.ImportName));
        return array;
    }

    public int Size {
        get { return this._imports.Count; }
    }
}