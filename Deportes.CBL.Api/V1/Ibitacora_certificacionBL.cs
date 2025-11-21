using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IBitacoraCertificacionBL : ICrudBL<bitacora_certificacionDTO, int>
    {
        Task<List<bitacora_certificacionDTO>> ObtenerPorFactura(int idFactura);
        Task<List<bitacora_certificacionDTO>> ObtenerPorUsuario(int idUsuario);
        Task<List<bitacora_certificacionDTO>> ObtenerPorTipoEvento(byte tipoEvento);
    }
}
