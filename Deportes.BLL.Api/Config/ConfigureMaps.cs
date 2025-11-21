using AutoMapper;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;

namespace Deportes.BLL.Api.Config
{
    public class ConfigureMapsProfile : Profile
    {
        public ConfigureMapsProfile()
        {
            CreateMap<bitacora_certificacionDTO, bitacora_certificacion>().ReverseMap();
            CreateMap<ClienteDTO, cliente>().ReverseMap();
            CreateMap<detalle_facturaDTO, detalle_factura>().ReverseMap();
            CreateMap<FacturaDTO, factura>().ReverseMap();
            CreateMap<ProductoDTO, producto>().ReverseMap();
            CreateMap<RolDTO, rol>().ReverseMap();
            CreateMap<UsuarioDTO, usuario>().ReverseMap();
        }
    }
}
