using AutoMapper;
using Deportes.CBL.Api.V1;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Deportes.BLL.Api
{
    public class BitacoraCertificacionBL : IBitacoraCertificacionBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public BitacoraCertificacionBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<bitacora_certificacionDTO>> ObtenerTodos()
        {
            var lista = await _context.bitacora_certificacion
                         .OrderByDescending(x => x.fecha_evento)
                         .ToListAsync();
            return _mapper.Map<List<bitacora_certificacionDTO>>(lista);
        }

        public async Task<bitacora_certificacionDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.bitacora_certificacion
                            .Where(x => x.id_bitacora == id)
                            .FirstOrDefaultAsync();
            return _mapper.Map<bitacora_certificacionDTO>(entidad);
        }

        public async Task<bitacora_certificacionDTO> Crear(bitacora_certificacionDTO modelo)
        {
            var nuevaEntidad = _mapper.Map<bitacora_certificacion>(modelo);

            // Si la fecha_evento no viene del front, asignar fecha actual
            if (modelo.fecha_evento == DateTime.MinValue)
                nuevaEntidad.fecha_evento = DateTime.Now;

            // Validar que exista la factura
            var facturaExiste = await _context.factura
                .AnyAsync(f => f.id_factura == modelo.id_factura);

            if (!facturaExiste)
                throw new ArgumentException($"La factura con ID {modelo.id_factura} no existe");

            // Validar que exista el usuario
            var usuarioExiste = await _context.usuario
                .AnyAsync(u => u.id_usuario == modelo.id_usuario);

            if (!usuarioExiste)
                throw new ArgumentException($"El usuario con ID {modelo.id_usuario} no existe");

            _context.bitacora_certificacion.Add(nuevaEntidad);
            await _context.SaveChangesAsync();

            return _mapper.Map<bitacora_certificacionDTO>(nuevaEntidad);
        }

        public async Task<bitacora_certificacionDTO> Actualizar(bitacora_certificacionDTO modelo)
        {
            var entidadExistente = await _context.bitacora_certificacion
                .FirstOrDefaultAsync(x => x.id_bitacora == modelo.id_bitacora);

            if (entidadExistente == null)
                throw new ArgumentException($"Bitácora con ID {modelo.id_bitacora} no encontrada");

            // Actualizar solo los campos permitidos
            entidadExistente.id_factura = modelo.id_factura;
            entidadExistente.tipo_evento = modelo.tipo_evento;
            entidadExistente.fecha_evento = modelo.fecha_evento;
            entidadExistente.id_usuario = modelo.id_usuario;
            entidadExistente.comentario = modelo.comentario;

            // Validaciones
            var facturaExiste = await _context.factura
                .AnyAsync(f => f.id_factura == modelo.id_factura);

            if (!facturaExiste)
                throw new ArgumentException($"La factura con ID {modelo.id_factura} no existe");

            var usuarioExiste = await _context.usuario
                .AnyAsync(u => u.id_usuario == modelo.id_usuario);

            if (!usuarioExiste)
                throw new ArgumentException($"El usuario con ID {modelo.id_usuario} no existe");

            _context.bitacora_certificacion.Update(entidadExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<bitacora_certificacionDTO>(entidadExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var entidad = await _context.bitacora_certificacion
                .FirstOrDefaultAsync(x => x.id_bitacora == id);

            if (entidad == null)
                throw new ArgumentException($"Bitácora con ID {id} no encontrada");

            // Eliminación física (ya que no hay campo estado)
            _context.bitacora_certificacion.Remove(entidad);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<List<bitacora_certificacionDTO>> ObtenerPorFactura(int idFactura)
        {
            var lista = await _context.bitacora_certificacion
                .Where(x => x.id_factura == idFactura)
                .OrderByDescending(x => x.fecha_evento)
                .ToListAsync();

            return _mapper.Map<List<bitacora_certificacionDTO>>(lista);
        }

        public async Task<List<bitacora_certificacionDTO>> ObtenerPorUsuario(int idUsuario)
        {
            var lista = await _context.bitacora_certificacion
                .Where(x => x.id_usuario == idUsuario)
                .OrderByDescending(x => x.fecha_evento)
                .ToListAsync();

            return _mapper.Map<List<bitacora_certificacionDTO>>(lista);
        }

        // Método adicional para obtener bitácoras por tipo de evento
        public async Task<List<bitacora_certificacionDTO>> ObtenerPorTipoEvento(byte tipoEvento)
        {
            var lista = await _context.bitacora_certificacion
                .Where(x => x.tipo_evento == tipoEvento)
                .OrderByDescending(x => x.fecha_evento)
                .ToListAsync();

            return _mapper.Map<List<bitacora_certificacionDTO>>(lista);
        }
    }

}