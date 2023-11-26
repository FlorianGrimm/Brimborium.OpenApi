namespace Brimborium.OpenApi.NgOpenApi;
public abstract class GenType {
    public string TypeName { get; set; }
    public string Namespace { get; set; }
    public string QualifiedName { get; set; }
    public string FileName { get; set; }
    public string TsComments { get; set; }
    public string PathToRoot { get; set; }
    public List<Import> Imports { get; set; }
    private Imports _imports;
    public List<string> AdditionalDependencies { get; set; }
    private HashSet<string> _additionalDependencies = new HashSet<string>();

    public GenType(string name, Func<string, Options, string> typeNameTransform, Options options) {
        this.TypeName = typeNameTransform(name, options);
        this.Namespace = Namespace(name); // Assuming Namespace is a method in your context
        this.FileName = FileName(this.TypeName); // Assuming FileName is a method in your context
        this.QualifiedName = this.TypeName;
        if (this.Namespace != null) {
            this.FileName = this.Namespace + "/" + this.FileName;
            this.QualifiedName = TypeName(this.Namespace, options) + this.TypeName; // Assuming TypeName is a method in your context
        }
        this._imports = new Imports(options); // Assuming Imports is a class in your context
    }

    protected void AddImport(string param) {
        if (param != null && !SkipImport(param)) {
            this._imports.Add(param); // Assuming Add is a method in your context
        }
    }

    protected abstract bool SkipImport(string name);

    protected abstract string InitPathToRoot();

    protected void UpdateImports() {
        this.PathToRoot = InitPathToRoot();
        this.Imports = this._imports.ToArray(); // Assuming ToArray is a method in your context
        foreach (var imp in this.Imports) {
            imp.Path = this.PathToRoot + imp.Path;
        }
        this.AdditionalDependencies = this._additionalDependencies.ToList();
    }

    protected void CollectImports(SchemaObject schema, bool additional = false, bool processOneOf = false) {
        /*
          protected collectImports(schema: SchemaObject | ReferenceObject | undefined, additional = false, processOneOf = false): void {
    if (!schema) {
      return;
    } else if (schema.$ref) {
      const dep = simpleName(schema.$ref);
      if (additional) {
        this._additionalDependencies.add(dep);
      } else {
        this.addImport(dep);
      }
    } else {
      schema = schema as SchemaObject;
      (schema.oneOf || []).forEach(i => this.collectImports(i, additional));
      (schema.allOf || []).forEach(i => this.collectImports(i, additional));
      (schema.anyOf || []).forEach(i => this.collectImports(i, additional));
      if (processOneOf) {
        (schema.oneOf || []).forEach(i => this.collectImports(i, additional));
      }
      if (schema.items) {
        this.collectImports(schema.items, additional);
      }
      if (schema.properties) {
        const properties = schema.properties;
        Object.keys(properties).forEach(p => {
          const prop = properties[p];
          this.collectImports(prop, additional, true);
        });
      }
      if (typeof schema.additionalProperties === 'object') {
        this.collectImports(schema.additionalProperties, additional);
      }
    }
  }
         */
    }
}