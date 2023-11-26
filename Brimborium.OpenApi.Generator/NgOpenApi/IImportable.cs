namespace Brimborium.OpenApi.NgOpenApi;

public interface IImportable {
    string ImportName { get; set; }
    string ImportPath { get; set; }
    string ImportFile { get; set; }
    string ImportTypeName { get; set; }
    string ImportQualifiedName { get; set; }
}
