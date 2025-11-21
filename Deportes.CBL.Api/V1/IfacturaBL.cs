using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IFacturaBL
    {
        // CRUD base
        Task<List<FacturaDTO>> ObtenerTodos();
        Task<FacturaDTO> ObtenerPorId(int id);
        Task<FacturaDTO> Crear(FacturaDTO modelo);
        Task<FacturaDTO> Actualizar(FacturaDTO modelo);
        Task<bool> Eliminar(int id);

        // Búsquedas / filtros
        Task<FacturaDTO> ObtenerPorUuid(Guid uuid);
        Task<FacturaDTO> ObtenerPorNumeroAutorizacion(string numeroAutorizacion);
        Task<FacturaDTO> ObtenerPorSerieCorrelativo(string serie, int correlativo);

        Task<List<FacturaDTO>> ObtenerPorClienteEmisor(int idClienteEmisor);
        Task<List<FacturaDTO>> ObtenerPorClienteReceptor(int idClienteReceptor);
        Task<List<FacturaDTO>> ObtenerPorUsuario(int idUsuario);
        Task<List<FacturaDTO>> ObtenerPorEstado(byte estado);

        Task<List<FacturaDTO>> ObtenerPorRangoFecha(DateTime desde, DateTime hasta);
        Task<List<FacturaDTO>> ObtenerEliminadas(bool eliminadas);

        // Acciones extra
        Task<bool> CambiarEstado(int id, byte nuevoEstado);
        Task<bool> AnularFactura(int id);
        Task<FacturaDTO> ClonarFactura(int id);
        Task<Dictionary<string, object>> ObtenerEstadisticas();
    }
}
