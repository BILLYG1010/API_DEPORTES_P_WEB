using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IRolBL : ICrudBL<RolDTO, int>
    {
        Task<RolDTO> ObtenerPorNombre(string nombre);
    }
}
