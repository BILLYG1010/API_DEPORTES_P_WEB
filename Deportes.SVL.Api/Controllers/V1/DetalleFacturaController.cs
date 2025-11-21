using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/detalles-factura")]
    public class DetalleFacturaController : ControllerBase
    {
        private readonly IDetalleFacturaBL _bl;

        public DetalleFacturaController(IDetalleFacturaBL bl)
        {
            _bl = bl;
        }

        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        [HttpGet("factura/{idFactura:int}")]
        public async Task<IActionResult> GetByFactura(int idFactura) =>
            Ok(await _bl.ObtenerPorFactura(idFactura));

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] detalle_facturaDTO dto) =>
            Ok(await _bl.Crear(dto));

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] detalle_facturaDTO dto)
        {
            dto.id_detalle = id;
            return Ok(await _bl.Actualizar(dto));
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));

        [HttpDelete("factura/{idFactura:int}")]
        public async Task<IActionResult> DeleteByFactura(int idFactura) =>
            Ok(await _bl.EliminarPorFactura(idFactura));

        [HttpGet("factura/{idFactura:int}/subtotal")]
        public async Task<IActionResult> GetSubtotalFactura(int idFactura) =>
            Ok(await _bl.CalcularSubtotalFactura(idFactura));

        [HttpGet("factura/{idFactura:int}/validar")]
        public async Task<IActionResult> ValidarDetalles(int idFactura) =>
            Ok(await _bl.ValidarDetallesFactura(idFactura));
    }
}
