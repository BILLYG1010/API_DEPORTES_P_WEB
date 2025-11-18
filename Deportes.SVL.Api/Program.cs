using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// 1. CORS básico (para desarrollo)
const string CORS_POLICY = "AllowAll";

builder.Services.AddCors(options =>
{
    options.AddPolicy(CORS_POLICY, policy =>
    {
        policy
            .AllowAnyOrigin()    // solo para DEV, luego lo puedes restringir
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// 2. MVC + Swagger
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Deportes API",
        Version = "v1"
    });
});

var app = builder.Build();

// 3. Swagger
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        // Swagger en /api-deportes
        c.RoutePrefix = "api-deportes";
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Deportes API v1");
    });
}

// 4. Pipeline HTTP
app.UseHttpsRedirection();
app.UseCors(CORS_POLICY);
app.UseAuthorization();
app.MapControllers();

app.Run();
