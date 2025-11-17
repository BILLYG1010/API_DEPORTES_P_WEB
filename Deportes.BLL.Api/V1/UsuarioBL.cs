using AutoMapper;
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

        public async Task<List<UsuarioDTO>> ObtenerTodos()
        {
            var lista = await _context.usuario
                         .Where(u => u.activo == 1)
                         .OrderBy(u => u.nombre_usuario)
                         .ToListAsync();
            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        public async Task<UsuarioDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.usuario
                            .Where(u => u.id_usuario == id && u.activo == 1)
                            .FirstOrDefaultAsync();
            return _mapper.Map<UsuarioDTO>(entidad);
        }

        public async Task<UsuarioDTO> ObtenerPorNombreUsuario(string nombreUsuario)
        {
            var entidad = await _context.usuario
                            .Where(u => u.nombre_usuario == nombreUsuario && u.activo == 1)
                            .FirstOrDefaultAsync();
            return _mapper.Map<UsuarioDTO>(entidad);
        }

        public async Task<UsuarioDTO> Crear(UsuarioDTO modelo)
        {
            // Validar que el nombre de usuario no exista
            var usuarioExistente = await _context.usuario
                .AnyAsync(u => u.nombre_usuario == modelo.nombre_usuario && u.activo == 1);

            if (usuarioExistente)
                throw new ArgumentException($"Ya existe un usuario con el nombre {modelo.nombre_usuario}");

            // Validar que el rol exista
            var rolExiste = await _context.rol
                .AnyAsync(r => r.id_rol == modelo.id_rol);

            if (!rolExiste)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no existe");

            var nuevoUsuario = _mapper.Map<usuario>(modelo);

            // Asignar valores por defecto
            nuevoUsuario.creado_en = DateTime.Now;
            nuevoUsuario.actualizado_en = DateTime.Now;
            nuevoUsuario.activo = 1;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nombre_usuario))
                throw new ArgumentException("El nombre de usuario es requerido");

            if (string.IsNullOrEmpty(modelo.password_hash))
                throw new ArgumentException("El password hash es requerido");

            _context.usuario.Add(nuevoUsuario);
            await _context.SaveChangesAsync();

            return _mapper.Map<UsuarioDTO>(nuevoUsuario);
        }

        public async Task<UsuarioDTO> Actualizar(UsuarioDTO modelo)
        {
            var usuarioExistente = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == modelo.id_usuario && u.activo == 1);

            if (usuarioExistente == null)
                throw new ArgumentException($"Usuario con ID {modelo.id_usuario} no encontrado");

            // Validar que el nombre de usuario no esté duplicado
            var usuarioDuplicado = await _context.usuario
                .AnyAsync(u => u.nombre_usuario == modelo.nombre_usuario && u.id_usuario != modelo.id_usuario && u.activo == 1);

            if (usuarioDuplicado)
                throw new ArgumentException($"Ya existe otro usuario con el nombre {modelo.nombre_usuario}");

            // Validar que el rol exista
            var rolExiste = await _context.rol
                .AnyAsync(r => r.id_rol == modelo.id_rol);

            if (!rolExiste)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no existe");

            // Actualizar campos
            usuarioExistente.nombre_usuario = modelo.nombre_usuario;
            usuarioExistente.password_hash = modelo.password_hash;
            usuarioExistente.id_rol = modelo.id_rol;
            usuarioExistente.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nombre_usuario))
                throw new ArgumentException("El nombre de usuario es requerido");

            if (string.IsNullOrEmpty(modelo.password_hash))
                throw new ArgumentException("El password hash es requerido");

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

            // Verificar si tiene facturas asociadas
            var tieneFacturas = await _context.factura
                .AnyAsync(f => f.id_usuario == id && f.eliminado == 0);

            if (tieneFacturas)
                throw new InvalidOperationException("No se puede eliminar el usuario porque tiene facturas asociadas");

            // Eliminación lógica
            usuario.activo = 0;
            usuario.actualizado_en = DateTime.Now;

            _context.usuario.Update(usuario);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<List<UsuarioDTO>> ObtenerUsuariosActivos()
        {
            var lista = await _context.usuario
                .Where(u => u.activo == 1)
                .OrderBy(u => u.nombre_usuario)
                .ToListAsync();

            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        public async Task<List<UsuarioDTO>> ObtenerUsuariosPorRol(int idRol)
        {
            var lista = await _context.usuario
                .Where(u => u.id_rol == idRol && u.activo == 1)
                .OrderBy(u => u.nombre_usuario)
                .ToListAsync();

            return _mapper.Map<List<UsuarioDTO>>(lista);
        }

        public async Task<bool> CambiarPassword(int id, string nuevoPasswordHash)
        {
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.id_usuario == id && u.activo == 1);

            if (usuario == null)
                throw new ArgumentException($"Usuario con ID {id} no encontrado");

            if (string.IsNullOrEmpty(nuevoPasswordHash))
                throw new ArgumentException("El nuevo password hash es requerido");

            usuario.password_hash = nuevoPasswordHash;
            usuario.actualizado_en = DateTime.Now;

            _context.usuario.Update(usuario);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<bool> ValidarCredenciales(string nombreUsuario, string passwordHash)
        {
            var usuario = await _context.usuario
                .FirstOrDefaultAsync(u => u.nombre_usuario == nombreUsuario && u.activo == 1);

            if (usuario == null)
                return false;

            return usuario.password_hash == passwordHash;
        }
    }

    public interface IUsuarioBL
    {
    }
}