using Deportes.BSV.Api.V1;
using Deportes.CSV.Api.V1;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using SoapCore;


namespace Deportes.BSV.Api.Config
{
    public static class ConfigureServices
    {
        public static IServiceCollection AddBSVConfig(this IServiceCollection services)
        {
            services.AddScoped<IDeportesServiceV1, DeportesServiceV1>();
            services.AddScoped<IDeportesProxyServiceV1, DeportesServiceV1>();
            return services;
        }

        // SOLO mapea endpoints SOAP (no pipeline)
        public static IEndpointRouteBuilder MapSoapEndpoints(this IEndpointRouteBuilder endpoints)
        {
            endpoints.UseSoapEndpoint<IDeportesProxyServiceV1>(
                "/DeportesService.svc",
                new SoapEncoderOptions(),
                SoapSerializer.DataContractSerializer
            );

            return endpoints;
        }
    }
}
