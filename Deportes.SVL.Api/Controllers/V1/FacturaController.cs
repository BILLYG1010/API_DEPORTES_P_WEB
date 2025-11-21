using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/facturas")]
    public class FacturaController : ControllerBase
    {
        private readonly IFacturaBL _bl;

        public FacturaController(IFacturaBL bl)
        {
            _bl = bl;
        }

        // GET: api/v1/facturas
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        // GET: api/v1/facturas/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        // GET: api/v1/facturas/uuid/{uuid}
        [HttpGet("uuid/{uuid:guid}")]
        public async Task<IActionResult> GetByUuid(Guid uuid) =>
            Ok(await _bl.ObtenerPorUuid(uuid));

        // POST: api/v1/facturas
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] FacturaDTO dto) =>
            Ok(await _bl.Crear(dto));

        // PUT: api/v1/facturas/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] FacturaDTO dto)
        {
            dto.id_factura = id;
            return Ok(await _bl.Actualizar(dto));
        }

        // DELETE: api/v1/facturas/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));

        // GET: api/v1/facturas/emisor/1
        [HttpGet("emisor/{idCliente:int}")]
        public async Task<IActionResult> GetByEmisor(int idCliente) =>
            Ok(await _bl.ObtenerPorClienteEmisor(idCliente));

        // GET: api/v1/facturas/receptor/1
        [HttpGet("receptor/{idCliente:int}")]
        public async Task<IActionResult> GetByReceptor(int idCliente) =>
            Ok(await _bl.ObtenerPorClienteReceptor(idCliente));

        // GET: api/v1/facturas/usuario/1
        [HttpGet("usuario/{idUsuario:int}")]
        public async Task<IActionResult> GetByUsuario(int idUsuario) =>
            Ok(await _bl.ObtenerPorUsuario(idUsuario));

        // 👇 Se quitó constraint :byte
        // GET: api/v1/facturas/estado/1
        [HttpGet("estado/{estado}")]
        public async Task<IActionResult> GetByEstado(byte estado) =>
            Ok(await _bl.ObtenerPorEstado(estado));

        // GET: api/v1/facturas/rango-fechas?fechaInicio=2025-01-01&fechaFin=2025-01-31
        [HttpGet("rango-fechas")]
        public async Task<IActionResult> GetByRangoFechas(
            [FromQuery] DateTime fechaInicio,
            [FromQuery] DateTime fechaFin) =>
            Ok(await _bl.ObtenerPorRangoFecha(fechaInicio, fechaFin));

        // 👇 Se quitó constraint :byte
        // PATCH: api/v1/facturas/5/estado/1
        [HttpPatch("{id:int}/estado/{nuevoEstado}")]
        public async Task<IActionResult> CambiarEstado(int id, byte nuevoEstado) =>
            Ok(await _bl.CambiarEstado(id, nuevoEstado));

        // PATCH: api/v1/facturas/5/anular
        [HttpPatch("{id:int}/anular")]
        public async Task<IActionResult> AnularFactura(int id) =>
            Ok(await _bl.AnularFactura(id));

        // POST: api/v1/facturas/5/clonar
        [HttpPost("{id:int}/clonar")]
        public async Task<IActionResult> ClonarFactura(int id) =>
            Ok(await _bl.ClonarFactura(id));

        // GET: api/v1/facturas/estadisticas
        [HttpGet("estadisticas")]
        public async Task<IActionResult> Estadisticas() =>
            Ok(await _bl.ObtenerEstadisticas());
    }
}
