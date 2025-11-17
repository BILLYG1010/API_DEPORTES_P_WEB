using AutoMapper;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deportes.BLL.Api
{
    public class FacturaBL : IFacturaBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public FacturaBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<FacturaDTO>> ObtenerTodas()
        {
            var lista = await _context.factura
                         .Where(f => f.eliminado == 0)
                         .OrderByDescending(f => f.fecha_emision)
                         .ToListAsync();
            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<FacturaDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.factura
                            .Where(f => f.id_factura == id && f.eliminado == 0)
                            .FirstOrDefaultAsync();
            return _mapper.Map<FacturaDTO>(entidad);
        }

        public async Task<FacturaDTO> ObtenerPorUuid(Guid uuid)
        {
            var entidad = await _context.factura
                            .Where(f => f.uuid == uuid && f.eliminado == 0)
                            .FirstOrDefaultAsync();
            return _mapper.Map<FacturaDTO>(entidad);
        }

        public async Task<FacturaDTO> Crear(FacturaDTO modelo)
        {
            // Validar cliente emisor
            var clienteEmisorExiste = await _context.cliente
                .AnyAsync(c => c.id_cliente == modelo.id_cliente_emisor && c.activo == 1);

            if (!clienteEmisorExiste)
                throw new ArgumentException($"Cliente emisor con ID {modelo.id_cliente_emisor} no existe o no está activo");

            // Validar cliente receptor
            var clienteReceptorExiste = await _context.cliente
                .AnyAsync(c => c.id_cliente == modelo.id_cliente_receptor && c.activo == 1);

            if (!clienteReceptorExiste)
                throw new ArgumentException($"Cliente receptor con ID {modelo.id_cliente_receptor} no existe o no está activo");

            // Validar usuario
            var usuarioExiste = await _context.usuario
                .AnyAsync(u => u.id_usuario == modelo.id_usuario);

            if (!usuarioExiste)
                throw new ArgumentException($"Usuario con ID {modelo.id_usuario} no existe");

            var nuevaFactura = _mapper.Map<factura>(modelo);

            // Asignar valores por defecto
            nuevaFactura.uuid = Guid.NewGuid();
            nuevaFactura.fecha_emision = DateTime.Now;
            nuevaFactura.eliminado = 0;

            // Validar cálculos
            if (modelo.subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a cero");

            if (modelo.total <= 0)
                throw new ArgumentException("El total debe ser mayor a cero");

            if (modelo.total < modelo.subtotal)
                throw new ArgumentException("El total no puede ser menor al subtotal");

            _context.factura.Add(nuevaFactura);
            await _context.SaveChangesAsync();

            return _mapper.Map<FacturaDTO>(nuevaFactura);
        }

        public async Task<FacturaDTO> Actualizar(FacturaDTO modelo)
        {
            var facturaExistente = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == modelo.id_factura && f.eliminado == 0);

            if (facturaExistente == null)
                throw new ArgumentException($"Factura con ID {modelo.id_factura} no encontrada");

            // Validar que no esté anulada
            if (facturaExistente.estado == 2)
                throw new InvalidOperationException("No se puede modificar una factura anulada");

            // Validar cliente emisor
            var clienteEmisorExiste = await _context.cliente
                .AnyAsync(c => c.id_cliente == modelo.id_cliente_emisor && c.activo == 1);

            if (!clienteEmisorExiste)
                throw new ArgumentException($"Cliente emisor con ID {modelo.id_cliente_emisor} no existe o no está activo");

            // Validar cliente receptor
            var clienteReceptorExiste = await _context.cliente
                .AnyAsync(c => c.id_cliente == modelo.id_cliente_receptor && c.activo == 1);

            if (!clienteReceptorExiste)
                throw new ArgumentException($"Cliente receptor con ID {modelo.id_cliente_receptor} no existe o no está activo");

            // Validar usuario
            var usuarioExiste = await _context.usuario
                .AnyAsync(u => u.id_usuario == modelo.id_usuario);

            if (!usuarioExiste)
                throw new ArgumentException($"Usuario con ID {modelo.id_usuario} no existe");

            // Actualizar campos permitidos
            facturaExistente.id_cliente_emisor = modelo.id_cliente_emisor;
            facturaExistente.id_cliente_receptor = modelo.id_cliente_receptor;
            facturaExistente.id_usuario = modelo.id_usuario;
            facturaExistente.estado = modelo.estado;
            facturaExistente.subtotal = modelo.subtotal;
            facturaExistente.total = modelo.total;
            facturaExistente.observaciones = modelo.observaciones;
            facturaExistente.numero_autorizacion = modelo.numero_autorizacion;
            facturaExistente.serie = modelo.serie;
            facturaExistente.correlativo = modelo.correlativo;

            // Validar cálculos
            if (modelo.subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a cero");

            if (modelo.total <= 0)
                throw new ArgumentException("El total debe ser mayor a cero");

            if (modelo.total < modelo.subtotal)
                throw new ArgumentException("El total no puede ser menor al subtotal");

            _context.factura.Update(facturaExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<FacturaDTO>(facturaExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == id && f.eliminado == 0);

            if (factura == null)
                throw new ArgumentException($"Factura con ID {id} no encontrada");

            // Verificar si tiene detalles de factura
            var tieneDetalles = await _context.detalle_factura
                .AnyAsync(d => d.id_factura == id);

            if (tieneDetalles)
                throw new InvalidOperationException("No se puede eliminar la factura porque tiene detalles asociados");

            // Eliminación lógica
            factura.eliminado = 1;
            _context.factura.Update(factura);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<List<FacturaDTO>> ObtenerPorClienteEmisor(int idClienteEmisor)
        {
            var lista = await _context.factura
                .Where(f => f.id_cliente_emisor == idClienteEmisor && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<List<FacturaDTO>> ObtenerPorClienteReceptor(int idClienteReceptor)
        {
            var lista = await _context.factura
                .Where(f => f.id_cliente_receptor == idClienteReceptor && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<List<FacturaDTO>> ObtenerPorUsuario(int idUsuario)
        {
            var lista = await _context.factura
                .Where(f => f.id_usuario == idUsuario && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<List<FacturaDTO>> ObtenerPorEstado(byte estado)
        {
            var lista = await _context.factura
                .Where(f => f.estado == estado && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<List<FacturaDTO>> ObtenerPorRangoFechas(DateTime fechaInicio, DateTime fechaFin)
        {
            var lista = await _context.factura
                .Where(f => f.fecha_emision >= fechaInicio &&
                           f.fecha_emision <= fechaFin &&
                           f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(lista);
        }

        public async Task<bool> CambiarEstado(int id, byte nuevoEstado)
        {
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == id && f.eliminado == 0);

            if (factura == null)
                throw new ArgumentException($"Factura con ID {id} no encontrada");

            factura.estado = nuevoEstado;
            _context.factura.Update(factura);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<bool> AnularFactura(int id)
        {
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == id && f.eliminado == 0);

            if (factura == null)
                throw new ArgumentException($"Factura con ID {id} no encontrada");

            if (factura.estado == 2)
                throw new InvalidOperationException("La factura ya está anulada");

            factura.estado = 2;
            _context.factura.Update(factura);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<FacturaDTO> ClonarFactura(int id)
        {
            var facturaOriginal = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == id && f.eliminado == 0);

            if (facturaOriginal == null)
                throw new ArgumentException($"Factura con ID {id} no encontrada");

            var nuevaFactura = new factura
            {
                uuid = Guid.NewGuid(),
                id_cliente_emisor = facturaOriginal.id_cliente_emisor,
                id_cliente_receptor = facturaOriginal.id_cliente_receptor,
                id_usuario = facturaOriginal.id_usuario,
                fecha_emision = DateTime.Now,
                estado = 0,
                eliminado = 0,
                subtotal = facturaOriginal.subtotal,
                total = facturaOriginal.total,
                observaciones = $"Copia de factura #{facturaOriginal.id_factura} - {facturaOriginal.observaciones}",
                numero_autorizacion = null,
                serie = facturaOriginal.serie,
                correlativo = null
            };

            _context.factura.Add(nuevaFactura);
            await _context.SaveChangesAsync();

            return _mapper.Map<FacturaDTO>(nuevaFactura);
        }

        public async Task<Dictionary<string, object>> ObtenerEstadisticas()
        {
            var estadisticas = new Dictionary<string, object>();

            var facturas = await _context.factura
                .Where(f => f.eliminado == 0)
                .ToListAsync();

            estadisticas.Add("total_facturas", facturas.Count);
            estadisticas.Add("total_ventas", facturas.Sum(f => f.total));
            estadisticas.Add("promedio_venta", facturas.Count > 0 ? facturas.Average(f => f.total) : 0);

            estadisticas.Add("facturas_pendientes", facturas.Count(f => f.estado == 0));
            estadisticas.Add("facturas_aprobadas", facturas.Count(f => f.estado == 1));
            estadisticas.Add("facturas_anuladas", facturas.Count(f => f.estado == 2));

            var inicioMes = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var finMes = inicioMes.AddMonths(1).AddDays(-1);
            var ventasMes = facturas
                .Where(f => f.fecha_emision >= inicioMes && f.fecha_emision <= finMes)
                .Sum(f => f.total);
            estadisticas.Add("ventas_mes_actual", ventasMes);

            return estadisticas;
        }
    }

    public interface IFacturaBL
    {
    }
}

