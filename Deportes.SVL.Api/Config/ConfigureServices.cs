using Asp.Versioning;
using Meso.SVL.Libreria.Infrastructure;
using Microsoft.AspNetCore.ResponseCompression;
using System.IO.Compression;
using System.Reflection;

namespace Meso.SVL.Libreria.Config
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddSVLConfig(this IServiceCollection services, string policyName)
        {
            //API Version
            services.ConfigureOptions<ConfigureSwaggerOptions>();
            services.AddApiVersioning(options =>
            {
                options.DefaultApiVersion = new ApiVersion(1, 0);
                options.AssumeDefaultVersionWhenUnspecified = true;
                options.ReportApiVersions = true;
            }).AddApiExplorer(options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });

            services.AddSwaggerGen(options =>
            {
                options.OperationFilter<SwaggerDefaultValues>();

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });

            //API configs
            services.Configure<RouteOptions>(options => { options.LowercaseUrls = true; });
            services.AddCors(options =>
            {
                options.AddPolicy(policyName,
                    builder => builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod());
            });

            services.AddResponseCompression();
            services.Configure<GzipCompressionProviderOptions>(option => { option.Level = CompressionLevel.Fastest; });

            return services;
        }
    }
}
