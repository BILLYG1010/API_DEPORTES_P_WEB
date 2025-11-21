using AutoMapper;
using Deportes.CBL.Api.V1;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deportes.BLL.Api
{
    public class RolBL : IRolBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public RolBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================
        // CRUD (ICrudBL)
        // ============================

        public async Task<List<RolDTO>> ObtenerTodos()
        {
            var lista = await _context.rol
                .AsNoTracking()
                .OrderBy(r => r.nombre)
                .ToListAsync();

            return _mapper.Map<List<RolDTO>>(lista);
        }

        public async Task<RolDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.rol
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.id_rol == id);

            if (entidad == null)
                throw new ArgumentException($"Rol con ID {id} no encontrado");

            return _mapper.Map<RolDTO>(entidad);
        }

        public async Task<RolDTO> ObtenerPorNombre(string nombre)
        {
            nombre = (nombre ?? "").Trim();

            var entidad = await _context.rol
                .AsNoTracking()
                .FirstOrDefaultAsync(r => r.nombre == nombre);

            if (entidad == null)
                throw new ArgumentException($"Rol con nombre '{nombre}' no encontrado");

            return _mapper.Map<RolDTO>(entidad);
        }

        public async Task<RolDTO> Crear(RolDTO modelo)
        {
            ValidarNombre(modelo.nombre);

            var nombreNormalizado = modelo.nombre.Trim();

            // nombre es UNIQUE en DB -> valida contra todos
            var nombreExistente = await _context.rol
                .AnyAsync(r => r.nombre == nombreNormalizado);

            if (nombreExistente)
                throw new ArgumentException($"Ya existe un rol con el nombre {nombreNormalizado}");

            var nuevoRol = _mapper.Map<rol>(modelo);

            nuevoRol.nombre = nombreNormalizado;
            nuevoRol.creado_en = DateTime.Now;
            nuevoRol.actualizado_en = DateTime.Now;

            _context.rol.Add(nuevoRol);
            await _context.SaveChangesAsync();

            return _mapper.Map<RolDTO>(nuevoRol);
        }

        public async Task<RolDTO> Actualizar(RolDTO modelo)
        {
            ValidarNombre(modelo.nombre);

            var rolExistente = await _context.rol
                .FirstOrDefaultAsync(r => r.id_rol == modelo.id_rol);

            if (rolExistente == null)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no encontrado");

            var nombreNormalizado = modelo.nombre.Trim();

            var nombreDuplicado = await _context.rol
                .AnyAsync(r => r.nombre == nombreNormalizado && r.id_rol != modelo.id_rol);

            if (nombreDuplicado)
                throw new ArgumentException($"Ya existe otro rol con el nombre {nombreNormalizado}");

            rolExistente.nombre = nombreNormalizado;
            rolExistente.actualizado_en = DateTime.Now;

            _context.rol.Update(rolExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<RolDTO>(rolExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var rolEntity = await _context.rol
                .FirstOrDefaultAsync(r => r.id_rol == id);

            if (rolEntity == null)
                throw new ArgumentException($"Rol con ID {id} no encontrado");

            // Validar si tiene usuarios activos asociados
            var tieneUsuarios = await _context.usuario
                .AnyAsync(u => u.id_rol == id && u.activo == 1);

            if (tieneUsuarios)
                throw new InvalidOperationException("No se puede eliminar el rol porque tiene usuarios asociados");

            _context.rol.Remove(rolEntity);
            return await _context.SaveChangesAsync() > 0;
        }

        // ============================
        // Helper privado
        // ============================
        private static void ValidarNombre(string nombre)
        {
            if (string.IsNullOrWhiteSpace(nombre))
                throw new ArgumentException("El nombre del rol es requerido");
        }
    }
}
