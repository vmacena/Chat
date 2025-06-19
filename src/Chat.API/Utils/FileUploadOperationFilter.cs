using Microsoft.AspNetCore.Http;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Chat.API.Utils;

public class FileUploadOperationFilter : IOperationFilter
{
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        const string formDataMimeType = "multipart/form-data";

        if (operation.RequestBody?.Content?.ContainsKey(formDataMimeType) != true)
            return;

        var parameters = context.MethodInfo.GetParameters();
        var hasFormFile = parameters.Any(p =>
            p.ParameterType.GetProperties().Any(prop => prop.PropertyType == typeof(IFormFile))
        );

        if (!hasFormFile)
            return;

        var schema = new OpenApiSchema
        {
            Type = "object",
            Properties = new Dictionary<string, OpenApiSchema>
            {
                ["File"] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "binary",
                    Description = "Arquivo para upload",
                },
                ["ReceiverId"] = new OpenApiSchema
                {
                    Type = "string",
                    Format = "uuid",
                    Description = "ID do destinatário",
                },
            },
            Required = new HashSet<string> { "File", "ReceiverId" },
        };

        operation.RequestBody.Content[formDataMimeType].Schema = schema;
    }
}
