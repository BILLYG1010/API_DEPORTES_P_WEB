using Asp.Versioning.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Meso.SVL.Libreria.Infrastructure
{
    public class ConfigureSwaggerOptions : IConfigureOptions<SwaggerGenOptions>
    {
        private readonly IApiVersionDescriptionProvider _provider;

        public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
        {
            _provider = provider;
        }

        public void Configure(SwaggerGenOptions options)
        {
            foreach (var description in _provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, InformacionVersionApi(description));
            }
        }

        public static OpenApiInfo InformacionVersionApi(ApiVersionDescription description)
        {
            var info = new OpenApiInfo()
            {
                Title = "API Tienda de Deportes.",
                Version = description.ApiVersion.ToString(),
                Description = "Documentacion API Deportes",
                Contact = new OpenApiContact() { Name = "Rocio Chaj" }
            };

            if (description.IsDeprecated)
            {
                info.Description += " Esta version de la API esta obsoleta";
            }

            return info;
        }
    }
}
