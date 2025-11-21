using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IClienteBL : ICrudBL<ClienteDTO, int>
    {
        Task<ClienteDTO> ObtenerPorNit(string nit);
        Task<List<ClienteDTO>> BuscarPorNombre(string nombre);
    }
}
