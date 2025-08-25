namespace ImageUpload.User
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.OpenApi.Models;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Reflection;

        public class FormDataOperationFilter : IOperationFilter
        {
            public void Apply(OpenApiOperation operation, OperationFilterContext context)
            {
                var formFileParams = context.MethodInfo.GetParameters()
                    .Where(p => p.ParameterType == typeof(IFormFile) ||
                                (p.ParameterType.IsGenericType &&
                                 p.ParameterType.GetGenericTypeDefinition() == typeof(IFormFileCollection)))
                    .ToList();

                if (!formFileParams.Any())
                    return;

                // Set request body content type to multipart/form-data
                operation.RequestBody = new OpenApiRequestBody
                {
                    Content = new Dictionary<string, OpenApiMediaType>
                    {
                        ["multipart/form-data"] = new OpenApiMediaType
                        {
                            Schema = new OpenApiSchema
                            {
                                Type = "object",
                                Properties = new Dictionary<string, OpenApiSchema>()
                            }
                        }
                    }
                };

                foreach (var param in formFileParams)
                {
                    var schema = new OpenApiSchema
                    {
                        Type = "string",
                        Format = "binary"  // This tells Swagger it's a file
                    };

                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(
                        param.Name,
                        schema
                    );
                }

                // Also add other [FromForm] string parameters
                var otherParams = context.MethodInfo.GetParameters()
                    .Where(p => p.ParameterType == typeof(string) &&
                                p.GetCustomAttribute<FromFormAttribute>() != null)
                    .ToList();

                foreach (var param in otherParams)
                {
                    operation.RequestBody.Content["multipart/form-data"].Schema.Properties.Add(
                        param.Name,
                        new OpenApiSchema { Type = "string" }
                    );
                }
            }
        }
    
}
