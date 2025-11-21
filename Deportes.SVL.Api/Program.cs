using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Deportes.BLL.Api.Config;   // <-- importante
using Deportes.DAL.Api;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// ============================
// 1. CORS
// ============================
const string CORS_POLICY = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// ============================
// 2. Versionado API
// ============================
builder.Services
    .AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = "'v'VVV";
        options.SubstituteApiVersionInUrl = true;
    });

// ============================
// 3. Capas
// ============================
builder.Services.AddBLLConfig();   // <-- ya registra BL + AutoMapper

// ============================
// 4. DbContext (elige 1)

// --- Si es MySQL Pomelo ---
var connectionString = builder.Configuration.GetConnectionString("DeportesConnection");
builder.Services.AddDbContext<DeportesContext>(options =>
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
);

// --- Si es SQL Server ---
// var connectionString = builder.Configuration.GetConnectionString("DeportesConnection");
// builder.Services.AddDbContext<DeportesContext>(options =>
//     options.UseSqlServer(connectionString)
// );

// ============================
// 5. MVC + Swagger
// ============================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

builder.Services.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// provider Swagger por versiones
var provider = app.Services.GetRequiredService<IApiVersionDescriptionProvider>();

// ============================
// 6. Swagger UI
// ============================
if (app.Environment.IsDevelopment())
{
    var route = "api-deportes";
    var documentName = "{documentName}";
    var serviceName = "DeportesAPI";

    app.UseSwagger(c =>
    {
        c.RouteTemplate = $"{route}/{documentName}/{serviceName}.json";
    });

    app.UseSwaggerUI(c =>
    {
        c.RoutePrefix = route;

        foreach (var desc in provider.ApiVersionDescriptions)
        {
            c.SwaggerEndpoint(
                $"/{route}/{desc.GroupName}/{serviceName}.json",
                desc.GroupName.ToUpperInvariant()
            );
        }
    });
}

// ============================
// 7. Pipeline
// ============================
app.UseHttpsRedirection();
app.UseCors(CORS_POLICY);
app.UseAuthorization();
app.MapControllers();
app.Run();


// ============================
// Swagger por versión
// ============================
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
            options.SwaggerDoc(description.GroupName, new OpenApiInfo
            {
                Title = "Deportes API",
                Version = description.ApiVersion.ToString()
            });
        }
    }
}
