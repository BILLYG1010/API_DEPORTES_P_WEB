using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/roles")]
    public class RolController : ControllerBase
    {
        private readonly IRolBL _bl;

        public RolController(IRolBL bl)
        {
            _bl = bl;
        }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        [HttpGet("nombre/{nombre}")]
        public async Task<IActionResult> GetByNombre(string nombre) =>
            Ok(await _bl.ObtenerPorNombre(nombre));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RolDTO dto) =>
            Ok(await _bl.Crear(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] RolDTO dto)
        {
            dto.id_rol = id;
            return Ok(await _bl.Actualizar(dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));
    }
}
