using Microsoft.OpenApi.Readers;

namespace Brimborium.OpenApi.NgOpenApi;
public class NgOpenApiGenerator {
    private readonly List<OpenApiDocument> _ListOpenApiDocument = new ();
    
    public NgOpenApiGenerator() {
    }

    public async Task ReadAsync(Stream stream, OpenApiReaderSettings? settings, CancellationToken cancellationToken) {

        var streamReader = new Microsoft.OpenApi.Readers.OpenApiStreamReader(settings);
        ReadResult readResult = await streamReader.ReadAsync(stream, cancellationToken);
        var openApiDiagnostic = readResult.OpenApiDiagnostic;
        var openApiDocument = readResult.OpenApiDocument;
        this.Add(openApiDocument);
    }

    public void Add(OpenApiDocument openApiDocument) {
        this._ListOpenApiDocument.Add(openApiDocument);
    }
}
