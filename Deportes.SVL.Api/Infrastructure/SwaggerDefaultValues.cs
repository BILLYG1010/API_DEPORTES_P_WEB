using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Linq;
using System.Text.Json;

namespace Meso.SVL.Libreria.Infrastructure
{
    /// <summary>
    /// Filtro que configura los valores por defecto y metadatos del Swagger.
    /// Este código es estándar y se reutiliza en todos los proyectos.
    /// </summary>
    public class SwaggerDefaultValues : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var apiDescription = context.ApiDescription;

            // Marca los endpoints obsoletos si aplica
            operation.Deprecated |= apiDescription.IsDeprecated();

            // Revisión de tipos de respuesta soportados
            foreach (var responseType in apiDescription.SupportedResponseTypes)
            {
                var responseKey = responseType.IsDefaultResponse
                    ? "default"
                    : responseType.StatusCode.ToString();

                if (!operation.Responses.TryGetValue(responseKey, out var response))
                    continue;

                foreach (var contentType in response.Content.Keys.ToList())
                {
                    if (responseType.ApiResponseFormats.All(x => x.MediaType != contentType))
                        response.Content.Remove(contentType);
                }
            }

            if (operation.Parameters == null)
                return;

            // Configura descripciones y valores por defecto de parámetros
            foreach (var parameter in operation.Parameters)
            {
                var description = apiDescription.ParameterDescriptions
                    .FirstOrDefault(p => p.Name == parameter.Name);

                if (description == null)
                    continue;

                parameter.Description ??= description.ModelMetadata?.Description;

                if (parameter.Schema.Default == null && description.DefaultValue != null)
                {
                    var json = JsonSerializer.Serialize(description.DefaultValue, description.ModelMetadata?.ModelType);
                    parameter.Schema.Default = OpenApiAnyFactory.CreateFromJson(json);
                }

                parameter.Required |= description.IsRequired;
            }
        }
    }
}
