using Deportes.BLL.Api;

using Deportes.DAL.Api;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.BLL.Api.Config
{
    public static class ConfigureServices
    {
        private const string DBCONNECTION = "DeportesConnection";

        public static IServiceCollection AddBLLConfig(this IServiceCollection services)
        {
            services.AddSqlServer<DeportesContext>($"Name={DBCONNECTION}");
            services.AddAutoMapper(typeof(ConfigureMapsProfile).Assembly);

            // Registra tus servicios BL aquí
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
