using Deportes.DTO.Api.Models;

namespace Deportes.CBL.Api.V1
{
    public interface IProductoBL
    {
        // CRUD
        Task<List<ProductoDTO>> ObtenerTodos();
        Task<ProductoDTO> ObtenerPorId(int id);
        Task<ProductoDTO> ObtenerPorSku(string sku);

        Task<ProductoDTO> Crear(ProductoDTO modelo);
        Task<ProductoDTO> Actualizar(ProductoDTO modelo);
        Task<bool> Eliminar(int id);

        // Filtros / búsquedas
        Task<List<ProductoDTO>> BuscarPorNombre(string nombre);
        Task<List<ProductoDTO>> ObtenerActivos(bool activos);

        // Extras
        Task<bool> ActualizarStock(int id, int cantidad);
        Task<bool> ActualizarPrecio(int id, decimal precio);
    }
}
