using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/bitacoras-certificacion")]
    public class BitacoraCertificacionController : ControllerBase
    {
        private readonly IBitacoraCertificacionBL _bl;

        public BitacoraCertificacionController(IBitacoraCertificacionBL bl)
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
        public async Task<IActionResult> Post([FromBody] bitacora_certificacionDTO dto) =>
            Ok(await _bl.Crear(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] bitacora_certificacionDTO dto)
        {
            dto.id_bitacora = id;
            return Ok(await _bl.Actualizar(dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));

        [HttpGet("factura/{idFactura:int}")]
        public async Task<IActionResult> GetByFactura(int idFactura) =>
            Ok(await _bl.ObtenerPorFactura(idFactura));

        [HttpGet("usuario/{idUsuario:int}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario) =>
            Ok(await _bl.ObtenerPorUsuario(idUsuario));

        // 👇 Se quitó el constraint :byte
        // GET: api/v1/bitacoras-certificacion/tipo/1
        [HttpGet("tipo/{tipoEvento}")]
        public async Task<IActionResult> GetByTipoEvento(byte tipoEvento) =>
            Ok(await _bl.ObtenerPorTipoEvento(tipoEvento));
    }
}
