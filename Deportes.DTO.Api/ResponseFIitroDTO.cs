namespace Deportes.DTO.Api
{
    /// <summary>
    /// Respuesta estándar para resultados con filtros/paginación.
    /// </summary>
    public class ResponseFiltroDTO<T>
    {
        public bool Success { get; set; } = true;

        public T? SingleResult { get; set; }

        public string DisplayMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public static ResponseFiltroDTO<T> Ok(T data, string msg = "")
        {
            return new ResponseFiltroDTO<T>
            {
                Success = true,
                SingleResult = data,
                DisplayMessage = msg
            };
        }

        public static ResponseFiltroDTO<T> Fail(string error, string msg = "")
        {
            return new ResponseFiltroDTO<T>
            {
                Success = false,
                SingleResult = default,
                DisplayMessage = msg,
                ErrorMessage = error
            };
        }
    }
}
