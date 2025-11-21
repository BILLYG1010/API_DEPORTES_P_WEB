using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IUsuarioBL
    {
        // CRUD
        Task<List<UsuarioDTO>> ObtenerTodos();
        Task<UsuarioDTO> ObtenerPorId(int id);
        Task<UsuarioDTO> ObtenerPorNombreUsuario(string nombreUsuario);

        Task<UsuarioDTO> Crear(UsuarioDTO modelo);
        Task<UsuarioDTO> Actualizar(UsuarioDTO modelo);

        Task<bool> Eliminar(int id);

        // Filtros / búsquedas
        Task<List<UsuarioDTO>> ObtenerPorRol(int idRol);
        Task<List<UsuarioDTO>> ObtenerActivos(bool activos);

        // Extras
        Task<bool> CambiarPassword(int id, string nuevoPasswordHash);
        Task<bool> ValidarCredenciales(string nombreUsuario, string passwordHash);
    }
}
