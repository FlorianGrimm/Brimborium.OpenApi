namespace Brimborium.OpenApi.NgOpenApi;

public class Import : IImportable {
    public string Name { get; set; }
    public string TypeName { get; set; }
    public string QualifiedName { get; set; }
    public string Path { get; set; }
    public string File { get; set; }
    public bool UseAlias { get; set; }
    public string FullPath { get; set; }

    // Fields from IImportable
    public string ImportName { get; set; }
    public string ImportPath { get; set; }
    public string ImportFile { get; set; }
    public string ImportTypeName { get; set; }
    public string ImportQualifiedName { get; set; }

    public Import(string name, string typeName, string qName, string path, string file) {
        Name = name;
        TypeName = typeName;
        QualifiedName = qName;
        UseAlias = TypeName != QualifiedName;
        Path = path;
        File = file;
        FullPath = $"{string.Join("/", Path.Split('/').Where(p => !string.IsNullOrEmpty(p)))}/{string.Join("/", File.Split('/').Where(p => !string.IsNullOrEmpty(p)))}";

        ImportName = name;
        ImportPath = path;
        ImportFile = file;
        ImportTypeName = typeName;
        ImportQualifiedName = qName;
    }
}
