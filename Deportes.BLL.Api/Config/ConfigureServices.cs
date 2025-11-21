using AutoMapper;
using Deportes.CBL.Api.V1;
using Microsoft.Extensions.DependencyInjection;

namespace Deportes.BLL.Api.Config
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddBLLConfig(this IServiceCollection services)
        {
            services.AddAutoMapper(typeof(ConfigureMapsProfile).Assembly);

            services.AddScoped<IBitacoraCertificacionBL, BitacoraCertificacionBL>();
            services.AddScoped<IClienteBL, ClienteBL>();
            services.AddScoped<IDetalleFacturaBL, DetalleFacturaBL>();
            services.AddScoped<IFacturaBL, FacturaBL>();
            services.AddScoped<IProductoBL, ProductoBL>();
            services.AddScoped<IRolBL, RolBL>();
            services.AddScoped<IUsuarioBL, UsuarioBL>();

            return services;
        }
    }
}
