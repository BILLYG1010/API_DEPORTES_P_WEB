namespace Deportes.DTO.Api
{
    /// <summary>
    /// Respuesta estándar para servicios (REST/SOAP).
    /// </summary>
    public class ResponseDTO<T>
    {
        public bool Success { get; set; } = true;

        // Puede ser DTO, List<DTO>, bool, etc.
        public T? SingleResult { get; set; }

        public string DisplayMessage { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        // Helpers opcionales (te simplifican BSV)
        public static ResponseDTO<T> Ok(T data, string msg = "")
        {
            return new ResponseDTO<T>
            {
                Success = true,
                SingleResult = data,
                DisplayMessage = msg
            };
        }

        public static ResponseDTO<T> Fail(string error, string msg = "")
        {
            return new ResponseDTO<T>
            {
                Success = false,
                SingleResult = default,
                DisplayMessage = msg,
                ErrorMessage = error
            };
        }
    }
}
