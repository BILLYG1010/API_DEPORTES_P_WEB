using Deportes.DTO.Api;
using Deportes.DTO.Api.Models;

namespace Deportes.CSV.Api.V1
{
    public interface IDeportesServiceV1
    {
        // CLIENTE
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientes();
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorId(int id);
        Task<ResponseDTO<ClienteDTO>> CrearCliente(ClienteDTO cliente);
        Task<ResponseDTO<ClienteDTO>> EditarCliente(ClienteDTO cliente);
        Task<ResponseDTO<bool>> EliminarCliente(int id);

        // PRODUCTO
        Task<ResponseDTO<List<ProductoDTO>>> ObtenerProductos();
        Task<ResponseDTO<ProductoDTO>> ObtenerProductoPorId(int id);
        Task<ResponseDTO<ProductoDTO>> CrearProducto(ProductoDTO producto);
        Task<ResponseDTO<ProductoDTO>> EditarProducto(ProductoDTO producto);
        Task<ResponseDTO<bool>> EliminarProducto(int id);

        // FACTURA
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturas();
        Task<ResponseDTO<FacturaDTO>> ObtenerFacturaPorId(int id);
        Task<ResponseDTO<FacturaDTO>> CrearFactura(FacturaDTO factura);
        Task<ResponseDTO<FacturaDTO>> EditarFactura(FacturaDTO factura);
        Task<ResponseDTO<bool>> EliminarFactura(int id);

        // DETALLE FACTURA
        Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesPorFactura(int idFactura);

        // BITACORA
        Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasPorFactura(int idFactura);
    }
}
