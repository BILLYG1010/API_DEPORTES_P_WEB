using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Deportes.DTO.Api
{
    // Versión base (opcional)
    public class ResponseDTO
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }

    // Versión genérica que usa tu interfaz
    public class ResponseDTO<T> : ResponseDTO
    {
        public T? Data { get; set; }

        public ResponseDTO() { }

        public ResponseDTO(T data)
        {
            Data = data;
            Success = true;
        }

        public static ResponseDTO<T> Ok(T data, string? message = null) =>
            new ResponseDTO<T>(data) { Message = message };

        public static ResponseDTO<T> Fail(string message, IEnumerable<string>? errors = null) =>
            new ResponseDTO<T> { Success = false, Message = message, Errors = errors is null ? new() : new List<string>(errors) };
    }
}
