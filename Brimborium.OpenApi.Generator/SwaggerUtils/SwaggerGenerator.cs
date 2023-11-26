﻿using Microsoft.Extensions.Hosting;

using System.Runtime.CompilerServices;

namespace Brimborium.OpenApi.Generator.SwaggerUtils;

/// <summary>
/// swagger:DocumentName - as defined in code otherwise run webapp.
/// swagger:OutputPath - output filepath otherwise Console.Out.
/// swagger:yaml true:yaml otherwise json.
/// swagger:serializeasv2 - true:v2 otherwise v3.
/// swagger:host - host in swagger.
/// swagger:basepath - basepath in swagger.
/// </summary>
public static class SwaggerGenerator {
    public static bool Generating(
        WebApplicationBuilder builder,
        SwaggerDocOptions? swaggerDocOptions = default) {
        swaggerDocOptions ??= new SwaggerDocOptions();
        builder.Configuration.GetSection("Swagger").Bind(swaggerDocOptions);

        if (swaggerDocOptions.Generate) {
            builder.Environment.EnvironmentName = "SwaggerGen";
            return true;
        } else {
            return false;
        }
    }

    public static bool Generate(
        WebApplication webApplication,
        SwaggerDocOptions? swaggerDocOptions = default,
        [CallerFilePath] string? callerFilePath = default
        ) {

        swaggerDocOptions ??= new SwaggerDocOptions();
        webApplication.Configuration.GetSection("Swagger").Bind(swaggerDocOptions);

        if (swaggerDocOptions.Generate) {

            CancellationTokenSource cts = new();
            _ = HostingAbstractionsHostExtensions.RunAsync(webApplication, cts.Token);
            // TODO
            // var generatorOptions = webApplication.Services.GetRequiredService<IOptions<SwaggerGeneratorOptions>>().Value;

            //var swaggerOptions = serviceProvider.GetRequiredService<IOptions<SwaggerOptions>>().Value;
            //var asyncSwaggerProvider = serviceProvider.GetRequiredService<IOptions<IAsyncSwaggerProvider>>().Value;


            var swaggerProvider = webApplication.Services.GetRequiredService<ISwaggerProvider>();
            var swagger = swaggerProvider.GetSwagger(
                documentName: swaggerDocOptions.DocumentName,
                host: string.IsNullOrEmpty(swaggerDocOptions.Host) ? null : swaggerDocOptions.Host,
                basePath: string.IsNullOrEmpty(swaggerDocOptions.Basepath) ? null : swaggerDocOptions.Basepath
                );
            
            var outputPath = string.IsNullOrEmpty(swaggerDocOptions.OutputPath)
                    ? null
                    //: Path.Combine(Directory.GetCurrentDirectory(), swaggerDocOptions.OutputPath)
                    : Path.Combine(Path.GetDirectoryName(callerFilePath) ?? Directory.GetCurrentDirectory(), swaggerDocOptions.OutputPath)
                    ;
            if (string.IsNullOrEmpty(outputPath)) {
                var swSwagger = Console.Out;
                IOpenApiWriter writer = swaggerDocOptions.Yaml
                    ? new OpenApiYamlWriter(swSwagger)
                    : new OpenApiJsonWriter(swSwagger);
                if (swaggerDocOptions.SerializeasV2) {
                    swagger.SerializeAsV2(writer);
                } else {
                    swagger.SerializeAsV3(writer);
                }
            } else {
                var sbSwagger = new StringBuilder();
                using (var swSwagger = new StringWriter(sbSwagger)) {
                    IOpenApiWriter writer = swaggerDocOptions.Yaml
                        ? new OpenApiYamlWriter(swSwagger)
                        : new OpenApiJsonWriter(swSwagger);
                    if (swaggerDocOptions.SerializeasV2) {
                        swagger.SerializeAsV2(writer);
                    } else {
                        swagger.SerializeAsV3(writer);
                    }
                }
                var contentNew = sbSwagger.ToString();
                string contentOld;
                {
                    try {
                        contentOld = File.ReadAllText(outputPath);
                    } catch (FileNotFoundException) {
                        contentOld = string.Empty;
                    }
                }
                if (string.Equals(contentOld, contentNew, StringComparison.Ordinal)) {
                    Console.Out.WriteLine($"Swagger {(swaggerDocOptions.SerializeasV2 ? "V2" : "V3")} {(swaggerDocOptions.Yaml ? "YAML" : "JSON")} is uptodate to {outputPath}.");
                } else {
                    File.WriteAllText(outputPath, contentNew);
                    Console.Out.WriteLine($"Swagger {(swaggerDocOptions.SerializeasV2 ? "V2" : "V3")} {(swaggerDocOptions.Yaml ? "YAML" : "JSON")} successfully written to {outputPath}.");
                }
            }
            cts.Cancel();
            return true;
        } else {
            return false;
        }
    }
}
