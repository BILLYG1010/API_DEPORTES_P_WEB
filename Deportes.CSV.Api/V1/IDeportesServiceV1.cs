using Deportes.DTO.Api;
using Deportes.DTO.Api.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Deportes.CSV.Api
{
    public interface IDeportesServiceV1
    {
        // Métodos para BitacoraCertificacion
        Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasCertificacion();
        Task<ResponseDTO<bitacora_certificacionDTO>> ObtenerBitacoraCertificacionPorId(int id);
        Task<ResponseDTO<bitacora_certificacionDTO>> CrearBitacoraCertificacion(bitacora_certificacionDTO bitacora);
        Task<ResponseDTO<bitacora_certificacionDTO>> ActualizarBitacoraCertificacion(bitacora_certificacionDTO bitacora);
        Task<ResponseDTO<bitacora_certificacionDTO>> EliminarBitacoraCertificacion(int id);
        Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasPorFactura(int idFactura);
        Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasPoXrUsuario(int idUsuario);

        // Métodos para Cliente
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientes();
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorId(int id);
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorNit(string nit);
        Task<ResponseDTO<ClienteDTO>> CrearCliente(ClienteDTO cliente);
        Task<ResponseDTO<ClienteDTO>> ActualizarCliente(ClienteDTO cliente);
        Task<ResponseDTO<bool>> EliminarCliente(int id);
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientesActivos();
        Task<ResponseDTO<List<ClienteDTO>>> BuscarClientesPorNombre(string nombre);
        Task<ResponseDTO<bool>> ActivarCliente(int id);
        Task<ResponseDTO<bool>> DesactivarCliente(int id);
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientesParaFacturacion();
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasClienteEmisor(int idCliente);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasClienteReceptor(int idCliente);
        Task<ResponseDTO<bool>> ValidarClienteParaFacturacion(int idCliente);
        Task<ResponseDTO<Dictionary<string, object>>> ObtenerEstadisticasCliente(int idCliente);

        // Métodos para DetalleFactura
        Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesFactura();
        Task<ResponseDTO<detalle_facturaDTO>> ObtenerDetalleFacturaPorId(int id);
        Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesPorFactura(int idFactura);
        Task<ResponseDTO<detalle_facturaDTO>> CrearDetalleFactura(detalle_facturaDTO detalle);
        Task<ResponseDTO<detalle_facturaDTO>> ActualizarDetalleFactura(detalle_facturaDTO detalle);
        Task<ResponseDTO<bool>> EliminarDetalleFactura(int id);
        Task<ResponseDTO<bool>> EliminarDetallesPorFactura(int idFactura);
        Task<ResponseDTO<decimal>> CalcularSubtotalFactura(int idFactura);
        Task<ResponseDTO<bool>> ValidarDetallesFactura(int idFactura);

        // Métodos para Factura
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturas();
        Task<ResponseDTO<FacturaDTO>> ObtenerFacturaPorId(int id);
        Task<ResponseDTO<FacturaDTO>> ObtenerFacturaPorUuid(Guid uuid);
        Task<ResponseDTO<FacturaDTO>> CrearFactura(FacturaDTO factura);
        Task<ResponseDTO<FacturaDTO>> ActualizarFactura(FacturaDTO factura);
        Task<ResponseDTO<bool>> EliminarFactura(int id);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasPorClienteEmisor(int idClienteEmisor);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasPorClienteReceptor(int idClienteReceptor);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasPorUsuario(int idUsuario);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasPorEstado(byte estado);
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturasPorRangoFechas(DateTime fechaInicio, DateTime fechaFin);
        Task<ResponseDTO<bool>> CambiarEstadoFactura(int id, byte nuevoEstado);
        Task<ResponseDTO<bool>> AnularFactura(int id);
        Task<ResponseDTO<FacturaDTO>> ClonarFactura(int id);
        Task<ResponseDTO<Dictionary<string, object>>> ObtenerEstadisticasFacturas();
        Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesDeFactura(int idFactura);
        Task<ResponseDTO<bool>> AgregarDetalleAFactura(detalle_facturaDTO detalle);
        Task<ResponseDTO<decimal>> RecalcularTotalesFactura(int idFactura);

        // Métodos para Producto
        Task<ResponseDTO<List<ProductoDTO>>> ObtenerProductos();
        Task<ResponseDTO<ProductoDTO>> ObtenerProductoPorId(int id);
        Task<ResponseDTO<ProductoDTO>> ObtenerProductoPorSku(string sku);
        Task<ResponseDTO<ProductoDTO>> CrearProducto(ProductoDTO producto);
        Task<ResponseDTO<ProductoDTO>> ActualizarProducto(ProductoDTO producto);
        Task<ResponseDTO<bool>> EliminarProducto(int id);
        Task<ResponseDTO<List<ProductoDTO>>> ObtenerProductosActivos();
        Task<ResponseDTO<List<ProductoDTO>>> BuscarProductosPorNombre(string nombre);
        Task<ResponseDTO<bool>> ActualizarStockProducto(int id, int cantidad);
        Task<ResponseDTO<bool>> ActualizarPrecioProducto(int id, decimal precio);

        // Métodos para Rol
        Task<ResponseDTO<List<RolDTO>>> ObtenerRoles();
        Task<ResponseDTO<RolDTO>> ObtenerRolPorId(int id);
        Task<ResponseDTO<RolDTO>> ObtenerRolPorNombre(string nombre);
        Task<ResponseDTO<RolDTO>> CrearRol(RolDTO rol);
        Task<ResponseDTO<RolDTO>> ActualizarRol(RolDTO rol);
        Task<ResponseDTO<bool>> EliminarRol(int id);

        // Métodos para Usuario
        Task<ResponseDTO<List<UsuarioDTO>>> ObtenerUsuarios();
        Task<ResponseDTO<UsuarioDTO>> ObtenerUsuarioPorId(int id);
        Task<ResponseDTO<UsuarioDTO>> ObtenerUsuarioPorNombre(string nombreUsuario);
        Task<ResponseDTO<UsuarioDTO>> CrearUsuario(UsuarioDTO usuario);
        Task<ResponseDTO<UsuarioDTO>> ActualizarUsuario(UsuarioDTO usuario);
        Task<ResponseDTO<bool>> EliminarUsuario(int id);
        Task<ResponseDTO<List<UsuarioDTO>>> ObtenerUsuariosActivos();
        Task<ResponseDTO<List<UsuarioDTO>>> ObtenerUsuariosPorRol(int idRol);
        Task<ResponseDTO<bool>> CambiarPasswordUsuario(int id, string nuevoPasswordHash);
        Task<ResponseDTO<bool>> ValidarCredencialesUsuario(string nombreUsuario, string passwordHash);
    }
}