using AutoMapper;
using Deportes.CBL.Api.V1;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deportes.BLL.Api
{
    public class UsuarioBL : IUsuarioBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public UsuarioBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================
        // CRUD (ICrudBL)
        // ============================

        public async Task<List<UsuarioDTO>> ObtenerTodos()
        {
            var lista = await _context.usuario
                .AsNoTracking()
                .Where(u => u.activo == 1)
                .OrderBy(u => u.nombre_usuario)
                .ToListAsync();

            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        public async Task<UsuarioDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.id_usuario == id && u.activo == 1);

            if (entidad == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            return _mapper.Map<UsuarioDTO>(entidad);
        }

        public async Task<UsuarioDTO> ObtenerPorNombreUsuario(string nombreUsuario)
        {
            nombreUsuario = (nombreUsuario ?? "").Trim();

            var entidad = await _context.usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.nombre_usuario == nombreUsuario && u.activo == 1);

            if (entidad == null)
                throw new ArgumentException($"Usuario con nombre '{nombreUsuario}' no encontrado");

            return _mapper.Map<UsuarioDTO>(entidad);
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            ValidarModelo(modelo);

            var nombreNorm = modelo.nombre_usuario.Trim();

            // nombre_usuario es UNIQUE en DB -> validar contra TODOS (activos e inactivos)
            var usuarioExistente = await _context.usuario
                .AnyAsync(u => u.nombre_usuario == nombreNorm);

            if (usuarioExistente)
                throw new ArgumentException($"Ya existe un usuario con el nombre {nombreNorm}");

            // Validar rol
            var rolExiste = await _context.rol.AnyAsync(r => r.id_rol == modelo.id_rol);
            if (!rolExiste)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no existe");

            var nuevoUsuario = _mapper.Map<usuario>(modelo);

            nuevoUsuario.nombre_usuario = nombreNorm;
            nuevoUsuario.creado_en = DateTime.Now;
            nuevoUsuario.actualizado_en = DateTime.Now;
            nuevoUsuario.activo = 1;

            _context.usuario.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(nuevoUsuario);
        }

        public async Task<UsuarioDTO> Actualizar(UsuarioDTO modelo)
        {
            ValidarModelo(modelo);

            var usuarioExistente = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == modelo.id_usuario && u.activo == 1);

            if (usuarioExistente == null)
                throw new ArgumentException($"Usuario con ID {modelo.id_usuario} no encontrado");

            var nombreNorm = modelo.nombre_usuario.Trim();

            // validar duplicado contra TODOS
            var usuarioDuplicado = await _context.usuario
                .AnyAsync(u => u.nombre_usuario == nombreNorm && u.id_usuario != modelo.id_usuario);

            if (usuarioDuplicado)
                throw new ArgumentException($"Ya existe otro usuario con el nombre {nombreNorm}");

            // Validar rol
            var rolExiste = await _context.rol.AnyAsync(r => r.id_rol == modelo.id_rol);
            if (!rolExiste)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no existe");

            usuarioExistente.nombre_usuario = nombreNorm;
            usuarioExistente.password_hash = modelo.password_hash;
            usuarioExistente.id_rol = modelo.id_rol;
            usuarioExistente.actualizado_en = DateTime.Now;

            _context.usuario.Update(usuarioExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(usuarioExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == id && u.activo == 1);

            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            // Verificar facturas asociadas
            var tieneFacturas = await _context.factura
                .AnyAsync(f => f.id_usuario == id && f.eliminado == 0);

            if (tieneFacturas)
                throw new InvalidOperationException("No se puede eliminar el usuario porque tiene facturas asociadas");

            // Eliminación lógica
            usuario.activo = 0;
            usuario.actualizado_en = DateTime.Now;

            _context.usuario.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        // ============================
        // Métodos de la interfaz IUsuarioBL
        // ============================

        public async Task<List<UsuarioDTO>> ObtenerPorRol(int idRol)
        {
            var lista = await _context.usuario
                .AsNoTracking()
                .Where(u => u.id_rol == idRol && u.activo == 1)
                .OrderBy(u => u.nombre_usuario)
                .ToListAsync();

            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        public async Task<List<UsuarioDTO>> ObtenerActivos(bool activos)
        {
            ulong val = activos ? 1ul : 0ul;

            var lista = await _context.usuario
                .AsNoTracking()
                .Where(u => u.activo == val)
                .OrderBy(u => u.nombre_usuario)
                .ToListAsync();

            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        // ============================
        // Métodos extra tuyos (se quedan)
        // ============================

        public async Task<bool> CambiarPassword(int id, string nuevoPasswordHash)
        {
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == id && u.activo == 1);

            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            if (string.IsNullOrWhiteSpace(nuevoPasswordHash))
                throw new ArgumentException("El nuevo password hash es requerido");

            usuario.password_hash = nuevoPasswordHash;
            usuario.actualizado_en = DateTime.Now;

            _context.usuario.Update(usuario);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> ValidarCredenciales(string nombreUsuario, string passwordHash)
        {
            nombreUsuario = (nombreUsuario ?? "").Trim();

            var usuario = await _context.usuario
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.nombre_usuario == nombreUsuario && u.activo == 1);

            if (usuario == null)
                return false;

            return usuario.password_hash == passwordHash;
        }

        // ============================
        // Helper privado
        // ============================

        private static void ValidarModelo(UsuarioDTO modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.nombre_usuario))
                throw new ArgumentException("El nombre de usuario es requerido");

            if (string.IsNullOrWhiteSpace(modelo.password_hash))
                throw new ArgumentException("El password hash es requerido");

            if (modelo.id_rol <= 0)
                throw new ArgumentException("El rol es requerido");
        }
    }
}
