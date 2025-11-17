using AutoMapper;
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

        public async Task<List<RolDTO>> ObtenerTodos()
        {
            var lista = await _context.rol
                         .OrderBy(r => r.nombre)
                         .ToListAsync();
            return _mapper.Map<List<RolDTO>>(lista);
        }

        public async Task<RolDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.rol
                            .Where(r => r.id_rol == id)
                            .FirstOrDefaultAsync();
            return _mapper.Map<RolDTO>(entidad);
        }

        public async Task<RolDTO> ObtenerPorNombre(string nombre)
        {
            var entidad = await _context.rol
                            .Where(r => r.nombre == nombre)
                            .FirstOrDefaultAsync();
            return _mapper.Map<RolDTO>(entidad);
        }

        public async Task<RolDTO> Crear(RolDTO modelo)
        {
            // Validar que el nombre no exista
            var nombreExistente = await _context.rol
                .AnyAsync(r => r.nombre == modelo.nombre);

            if (nombreExistente)
                throw new ArgumentException($"Ya existe un rol con el nombre {modelo.nombre}");

            var nuevoRol = _mapper.Map<rol>(modelo);

            // Asignar valores por defecto
            nuevoRol.creado_en = DateTime.Now;
            nuevoRol.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre del rol es requerido");

            _context.rol.Add(nuevoRol);
            await _context.SaveChangesAsync();

            return _mapper.Map<RolDTO>(nuevoRol);
        }

        public async Task<RolDTO> Actualizar(RolDTO modelo)
        {
            var rolExistente = await _context.rol
                .FirstOrDefaultAsync(r => r.id_rol == modelo.id_rol);

            if (rolExistente == null)
                throw new ArgumentException($"Rol con ID {modelo.id_rol} no encontrado");

            // Validar que el nombre no esté duplicado
            var nombreDuplicado = await _context.rol
                .AnyAsync(r => r.nombre == modelo.nombre && r.id_rol != modelo.id_rol);

            if (nombreDuplicado)
                throw new ArgumentException($"Ya existe otro rol con el nombre {modelo.nombre}");

            // Actualizar campos
            rolExistente.nombre = modelo.nombre;
            rolExistente.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre del rol es requerido");

            _context.rol.Update(rolExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<RolDTO>(rolExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var rol = await _context.rol
                .FirstOrDefaultAsync(r => r.id_rol == id);

            if (rol == null)
                throw new ArgumentException($"Rol con ID {id} no encontrado");

            // Verificar si tiene usuarios asociados
            var tieneUsuarios = await _context.usuario
                .AnyAsync(u => u.id_rol == id && u.activo == 1);

            if (tieneUsuarios)
                throw new InvalidOperationException("No se puede eliminar el rol porque tiene usuarios asociados");

            _context.rol.Remove(rol);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }
    }

    public interface IRolBL
    {
    }
}