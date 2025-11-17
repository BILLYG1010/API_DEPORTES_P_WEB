using Deportes.SVL.Api.Config;
using Deportes.BLL.Api.Config;
using Deportes.DAL.Api;
using Deportes.BSV.Api.Config;





using System.Reflection;
using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);
const string CORS_POLICY = "CorsPolicy";
var corsValue = builder.Configuration.GetSection(CORS_POLICY).Value;

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

// Usar la configuración centralizada que registra ApiVersioning y Swagger
builder.Services.AddSVLConfig("AllowAll");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    var route = "api-deportes";
    var serviceName = "DeportesAPI";

    // Exponer los JSON de Swagger en /api-deportes/{documentName}/DeportesAPI.json
    app.UseSwagger(options =>
    {
        options.RouteTemplate = $"{route}/{{documentName}}/{serviceName}.json";
    });

    // UI en /api-deportes con un endpoint por cada versión registrada
    app.UseSwaggerUI(options =>
    {
        options.RoutePrefix = route;

        var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();
        foreach (var description in provider.ApiVersionDescriptions)
        {
            options.SwaggerEndpoint($"/{route}/{description.GroupName}/{serviceName}.json",
                                   description.GroupName.ToUpperInvariant());
        }

        options.DocumentTitle = "Deportes API Documentation";
        options.EnableDeepLinking();
        options.DisplayRequestDuration();
        options.DefaultModelsExpandDepth(-1);
    });
}

app.UseHttpsRedirection();
app.UseCors(corsValue);

app.UseAuthorization();

app.MapControllers();

app.Run();
