using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/productos")]
    public class ProductoController : ControllerBase
    {
        private readonly IProductoBL _bl;

        public ProductoController(IProductoBL bl)
        {
            _bl = bl;
        }

        // GET: api/v1/productos
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        // GET: api/v1/productos/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        // GET: api/v1/productos/sku/ABC123
        [HttpGet("sku/{sku}")]
        public async Task<IActionResult> GetBySku(string sku) =>
            Ok(await _bl.ObtenerPorSku(sku));

        // GET: api/v1/productos/activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos() =>
            Ok(await _bl.ObtenerActivos(true));

        // (Opcional) GET: api/v1/productos/inactivos
        [HttpGet("inactivos")]
        public async Task<IActionResult> GetInactivos() =>
            Ok(await _bl.ObtenerActivos(false));

        // GET: api/v1/productos/buscar?nombre=balon
        [HttpGet("buscar")]
        public async Task<IActionResult> BuscarPorNombre([FromQuery] string nombre) =>
            Ok(await _bl.BuscarPorNombre(nombre));

        // POST: api/v1/productos
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ProductoDTO dto) =>
            Ok(await _bl.Crear(dto));

        // PUT: api/v1/productos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] ProductoDTO dto)
        {
            dto.id_producto = id;
            return Ok(await _bl.Actualizar(dto));
        }

        // DELETE: api/v1/productos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));

        // PATCH: api/v1/productos/5/stock/100
        [HttpPatch("{id:int}/stock/{cantidad:int}")]
        public async Task<IActionResult> ActualizarStock(int id, int cantidad) =>
            Ok(await _bl.ActualizarStock(id, cantidad));

        // PATCH: api/v1/productos/5/precio/25.50
        [HttpPatch("{id:int}/precio/{precio:decimal}")]
        public async Task<IActionResult> ActualizarPrecio(int id, decimal precio) =>
            Ok(await _bl.ActualizarPrecio(id, precio));
    }
}
