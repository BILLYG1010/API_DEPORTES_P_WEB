using Deportes.DTO.Api;
using Deportes.DTO.Api.Models;
using System.ServiceModel;

namespace Deportes.CSV.Api.V1
{
    [ServiceContract(Name = "DeportesService")]
    public interface IDeportesProxyServiceV1
    {
        // CLIENTE
        [OperationContract(Name = "ObtenerClientes")]
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientes();

        [OperationContract(Name = "ObtenerClientePorId")]
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorId(int id);

        [OperationContract(Name = "CrearCliente")]
        Task<ResponseDTO<ClienteDTO>> CrearCliente(ClienteDTO cliente);

        [OperationContract(Name = "EditarCliente")]
        Task<ResponseDTO<ClienteDTO>> EditarCliente(ClienteDTO cliente);

        [OperationContract(Name = "EliminarCliente")]
        Task<ResponseDTO<bool>> EliminarCliente(int id);


        // PRODUCTO
        [OperationContract(Name = "ObtenerProductos")]
        Task<ResponseDTO<List<ProductoDTO>>> ObtenerProductos();

        [OperationContract(Name = "ObtenerProductoPorId")]
        Task<ResponseDTO<ProductoDTO>> ObtenerProductoPorId(int id);

        [OperationContract(Name = "CrearProducto")]
        Task<ResponseDTO<ProductoDTO>> CrearProducto(ProductoDTO producto);

        [OperationContract(Name = "EditarProducto")]
        Task<ResponseDTO<ProductoDTO>> EditarProducto(ProductoDTO producto);

        [OperationContract(Name = "EliminarProducto")]
        Task<ResponseDTO<bool>> EliminarProducto(int id);


        // FACTURA
        [OperationContract(Name = "ObtenerFacturas")]
        Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturas();

        [OperationContract(Name = "ObtenerFacturaPorId")]
        Task<ResponseDTO<FacturaDTO>> ObtenerFacturaPorId(int id);

        [OperationContract(Name = "CrearFactura")]
        Task<ResponseDTO<FacturaDTO>> CrearFactura(FacturaDTO factura);

        [OperationContract(Name = "EditarFactura")]
        Task<ResponseDTO<FacturaDTO>> EditarFactura(FacturaDTO factura);

        [OperationContract(Name = "EliminarFactura")]
        Task<ResponseDTO<bool>> EliminarFactura(int id);

        // DETALLE FACTURA
        [OperationContract(Name = "ObtenerDetallesPorFactura")]
        Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesPorFactura(int idFactura);

        // BITACORA
        [OperationContract(Name = "ObtenerBitacorasPorFactura")]
        Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasPorFactura(int idFactura);
    }
}
