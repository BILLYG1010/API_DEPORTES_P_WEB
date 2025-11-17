csharp Deportes.DTO.Api\ResponseDTO.cs
using System;
using System.Collections.Generic;

namespace Deportes.DTO.Api
{
    public class ResponseDTO
    {
        public bool Success { get; set; } = true;
        public string? Message { get; set; }
        public List<string> Errors { get; set; } = new();
    }

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