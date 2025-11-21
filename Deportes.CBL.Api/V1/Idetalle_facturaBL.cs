using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IDetalleFacturaBL
    {
        Task<List<detalle_facturaDTO>> ObtenerTodos();
        Task<detalle_facturaDTO> ObtenerPorId(int id);
        Task<List<detalle_facturaDTO>> ObtenerPorFactura(int idFactura);

        Task<detalle_facturaDTO> Crear(detalle_facturaDTO modelo);
        Task<detalle_facturaDTO> Actualizar(detalle_facturaDTO modelo);

        Task<bool> Eliminar(int id);
        Task<bool> EliminarPorFactura(int idFactura);

        Task<decimal> CalcularSubtotalFactura(int idFactura);
        Task<bool> ValidarDetallesFactura(int idFactura);
    }
}
