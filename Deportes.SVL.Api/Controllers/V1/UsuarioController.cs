using Asp.Versioning;
using Deportes.CBL.Api.V1;
using Deportes.DTO.Api.Models;
using Microsoft.AspNetCore.Mvc;

namespace Deportes.SVL.Api.Controllers.V1
{
    [ApiController]
    [ApiVersion("1.0")]
    [Route("api/v{version:apiVersion}/usuarios")]
    public class UsuarioController : ControllerBase
    {
        private readonly IUsuarioBL _bl;

        public UsuarioController(IUsuarioBL bl)
        {
            _bl = bl;
        }

        // GET: api/v1/usuarios
        [HttpGet]
        public async Task<IActionResult> Get() =>
            Ok(await _bl.ObtenerTodos());

        // GET: api/v1/usuarios/5
        [HttpGet("{id:int}")]
        public async Task<IActionResult> GetById(int id) =>
            Ok(await _bl.ObtenerPorId(id));

        // GET: api/v1/usuarios/nombre-usuario/billy
        [HttpGet("nombre-usuario/{nombreUsuario}")]
        public async Task<IActionResult> GetByNombreUsuario(string nombreUsuario) =>
            Ok(await _bl.ObtenerPorNombreUsuario(nombreUsuario));

        // GET: api/v1/usuarios/activos
        [HttpGet("activos")]
        public async Task<IActionResult> GetActivos() =>
            Ok(await _bl.ObtenerActivos(true));

        // (Opcional) GET: api/v1/usuarios/inactivos
        [HttpGet("inactivos")]
        public async Task<IActionResult> GetInactivos() =>
            Ok(await _bl.ObtenerActivos(false));

        // GET: api/v1/usuarios/rol/2
        [HttpGet("rol/{idRol:int}")]
        public async Task<IActionResult> GetByRol(int idRol) =>
            Ok(await _bl.ObtenerPorRol(idRol));

        // POST: api/v1/usuarios
        [HttpPost]
        public async Task<IActionResult> Post([FromBody] UsuarioDTO dto) =>
            Ok(await _bl.Crear(dto));

        // PUT: api/v1/usuarios/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Put(int id, [FromBody] UsuarioDTO dto)
        {
            dto.id_usuario = id;
            return Ok(await _bl.Actualizar(dto));
        }

        // DELETE: api/v1/usuarios/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Delete(int id) =>
            Ok(await _bl.Eliminar(id));

        // PATCH: api/v1/usuarios/5/password
        // Body: "nuevoHashAquí"
        [HttpPatch("{id:int}/password")]
        public async Task<IActionResult> CambiarPassword(int id, [FromBody] string nuevoPasswordHash) =>
            Ok(await _bl.CambiarPassword(id, nuevoPasswordHash));

        // POST: api/v1/usuarios/validar-credenciales
        // Body: { "nombre_usuario": "...", "password_hash": "..." }
        [HttpPost("validar-credenciales")]
        public async Task<IActionResult> ValidarCredenciales([FromBody] UsuarioDTO dto) =>
            Ok(await _bl.ValidarCredenciales(dto.nombre_usuario, dto.password_hash));
    }
}
