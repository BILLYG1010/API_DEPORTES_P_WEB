using AutoMapper;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.BLL.Api.Config
{
    public class ConfigureMapsProfile : Profile
    {
        public ConfigureMapsProfile()
        {
            // Mapeos entre entidades y DTOs - usando los nombres exactos de tus entidades
            CreateMap<bitacora_certificacionDTO, bitacora_certificacion>().ReverseMap();
            CreateMap<ClienteDTO, cliente>().ReverseMap();
            CreateMap<detalle_facturaDTO, detalle_factura>().ReverseMap();
            CreateMap<FacturaDTO, factura>().ReverseMap();
            CreateMap<ProductoDTO, producto>().ReverseMap();
            CreateMap<RolDTO, rol>().ReverseMap();
            CreateMap<UsuarioDTO, usuario>().ReverseMap();

            // Si tienes Class1.cs como entidad, agrega su mapeo aquí
            // CreateMap<Class1DTO, class1>().ReverseMap();
        }
    }
}
