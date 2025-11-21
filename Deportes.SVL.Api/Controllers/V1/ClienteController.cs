using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/clientes")]
    public class ClienteController : ControllerBase
    {
        private readonly IClienteBL _bl;

        public ClienteController(IClienteBL bl)
        {
            _bl = bl;
        }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ClienteDTO dto) =>
            Ok(await _bl.Crear(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ClienteDTO dto)
        {
            dto.id_cliente = id;
            return Ok(await _bl.Actualizar(dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));
    }
}
