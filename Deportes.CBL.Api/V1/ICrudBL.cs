namespace Deportes.CBL.Api.V1
{
    public interface ICrudBL<TDto, TKey>
    {
        Task<List<TDto>> ObtenerTodos();
        Task<TDto> ObtenerPorId(TKey id);
        Task<TDto> Crear(TDto modelo);
        Task<TDto> Actualizar(TDto modelo);
        Task<bool> Eliminar(TKey id);
    }
}
