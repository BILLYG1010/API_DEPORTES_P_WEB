using AutoMapper;
using Deportes.CBL.Api.V1;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Deportes.BLL.Api
{
    public class DetalleFacturaBL : IDetalleFacturaBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public DetalleFacturaBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        // ============================
        // CRUD
        // ============================

        public async Task<List<detalle_facturaDTO>> ObtenerTodos()
        {
            var lista = await _context.detalle_factura
                .AsNoTracking()
                .OrderBy(d => d.id_detalle)
                .ToListAsync();

            return _mapper.Map<List<detalle_facturaDTO>>(lista);
        }

        public async Task<detalle_facturaDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.detalle_factura
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.id_detalle == id);

            if (entidad == null)
                throw new ArgumentException($"Detalle con ID {id} no encontrado");

            return _mapper.Map<detalle_facturaDTO>(entidad);
        }

        public async Task<List<detalle_facturaDTO>> ObtenerPorFactura(int idFactura)
        {
            var lista = await _context.detalle_factura
                .AsNoTracking()
                .Where(d => d.id_factura == idFactura)
                .OrderBy(d => d.id_detalle)
                .ToListAsync();

            return _mapper.Map<List<detalle_facturaDTO>>(lista);
        }

        // ✅ requerido por la interfaz
        public async Task<List<detalle_facturaDTO>> ObtenerPorProducto(int idProducto)
        {
            var lista = await _context.detalle_factura
                .AsNoTracking()
                .Where(d => d.id_producto == idProducto)
                .OrderBy(d => d.id_detalle)
                .ToListAsync();

            return _mapper.Map<List<detalle_facturaDTO>>(lista);
        }

        public async Task<detalle_facturaDTO> Crear(detalle_facturaDTO modelo)
        {
            // Validar que la factura exista y no esté eliminada
            var facturaExiste = await _context.factura
                .AnyAsync(f => f.id_factura == modelo.id_factura && f.eliminado == 0);

            if (!facturaExiste)
                throw new ArgumentException($"Factura con ID {modelo.id_factura} no existe o está eliminada");

            // Validar que la factura no esté anulada
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == modelo.id_factura);

            if (factura?.estado == 2) // Estado 2 = Anulada
                throw new InvalidOperationException("No se pueden agregar detalles a una factura anulada");

            // Si tiene producto, validar que exista
            if (modelo.id_producto.HasValue)
            {
                var productoExiste = await _context.producto
                    .AnyAsync(p => p.id_producto == modelo.id_producto.Value);

                if (!productoExiste)
                    throw new ArgumentException($"Producto con ID {modelo.id_producto} no existe");
            }

            // Validaciones de negocio
            ValidarModelo(modelo);

            var nuevoDetalle = _mapper.Map<detalle_factura>(modelo);

            // Asignar fechas
            nuevoDetalle.creado_en = DateTime.Now;
            nuevoDetalle.actualizado_en = DateTime.Now;

            _context.detalle_factura.Add(nuevoDetalle);
            await _context.SaveChangesAsync();

            // Recalcular totales de la factura
            await RecalcularTotalesFactura(modelo.id_factura);

            return _mapper.Map<detalle_facturaDTO>(nuevoDetalle);
        }

        public async Task<detalle_facturaDTO> Actualizar(detalle_facturaDTO modelo)
        {
            var detalleExistente = await _context.detalle_factura
                .FirstOrDefaultAsync(d => d.id_detalle == modelo.id_detalle);

            if (detalleExistente == null)
                throw new ArgumentException($"Detalle con ID {modelo.id_detalle} no encontrado");

            var facturaAnteriorId = detalleExistente.id_factura;

            // Validar que la factura actual no esté anulada
            var facturaActual = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == detalleExistente.id_factura);

            if (facturaActual?.estado == 2)
                throw new InvalidOperationException("No se puede modificar un detalle de factura anulada");

            // Si cambia la factura, validar la nueva
            if (facturaAnteriorId != modelo.id_factura)
            {
                var nuevaFacturaExiste = await _context.factura
                    .AnyAsync(f => f.id_factura == modelo.id_factura && f.eliminado == 0);

                if (!nuevaFacturaExiste)
                    throw new ArgumentException($"Factura con ID {modelo.id_factura} no existe o está eliminada");

                var nuevaFactura = await _context.factura
                    .FirstOrDefaultAsync(f => f.id_factura == modelo.id_factura);

                if (nuevaFactura?.estado == 2)
                    throw new InvalidOperationException("No se pueden mover detalles a una factura anulada");
            }

            // Si tiene producto, validar que exista
            if (modelo.id_producto.HasValue)
            {
                var productoExiste = await _context.producto
                    .AnyAsync(p => p.id_producto == modelo.id_producto.Value);

                if (!productoExiste)
                    throw new ArgumentException($"Producto con ID {modelo.id_producto} no existe");
            }

            // Validaciones de negocio
            ValidarModelo(modelo);

            // Actualizar campos
            detalleExistente.id_factura = modelo.id_factura;
            detalleExistente.id_producto = modelo.id_producto;
            detalleExistente.descripcion = modelo.descripcion;
            detalleExistente.cantidad = modelo.cantidad;
            detalleExistente.precio_unitario = modelo.precio_unitario;
            detalleExistente.descuento = modelo.descuento;
            detalleExistente.subtotal = modelo.subtotal;
            detalleExistente.actualizado_en = DateTime.Now;

            _context.detalle_factura.Update(detalleExistente);
            await _context.SaveChangesAsync();

            // Recalcular totales de la factura nueva
            await RecalcularTotalesFactura(modelo.id_factura);

            // ✅ Si cambió de factura, recalcular también la anterior
            if (facturaAnteriorId != modelo.id_factura)
                await RecalcularTotalesFactura(facturaAnteriorId);

            return _mapper.Map<detalle_facturaDTO>(detalleExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var detalle = await _context.detalle_factura
                .FirstOrDefaultAsync(d => d.id_detalle == id);

            if (detalle == null)
                throw new ArgumentException($"Detalle con ID {id} no encontrado");

            // Validar que la factura no esté anulada
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == detalle.id_factura);

            if (factura?.estado == 2)
                throw new InvalidOperationException("No se puede eliminar un detalle de factura anulada");

            var idFactura = detalle.id_factura;

            _context.detalle_factura.Remove(detalle);
            var resultado = await _context.SaveChangesAsync();

            if (resultado > 0)
                await RecalcularTotalesFactura(idFactura);

            return resultado > 0;
        }

        // ============================
        // Métodos extra (útiles)
        // ============================

        public async Task<bool> EliminarPorFactura(int idFactura)
        {
            var detalles = await _context.detalle_factura
                .Where(d => d.id_factura == idFactura)
                .ToListAsync();

            if (!detalles.Any())
                return true;

            // Validar que factura no esté anulada
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == idFactura);

            if (factura?.estado == 2)
                throw new InvalidOperationException("No se puede eliminar detalles de una factura anulada");

            _context.detalle_factura.RemoveRange(detalles);
            var resultado = await _context.SaveChangesAsync();

            if (resultado > 0)
                await RecalcularTotalesFactura(idFactura);

            return resultado > 0;
        }

        public async Task<decimal> CalcularSubtotalFactura(int idFactura)
        {
            var subtotal = await _context.detalle_factura
                .Where(d => d.id_factura == idFactura)
                .Select(d => (decimal?)d.subtotal)
                .SumAsync();

            return subtotal ?? 0m;
        }

        public async Task<bool> ValidarDetallesFactura(int idFactura)
        {
            return await _context.detalle_factura
                .AnyAsync(d => d.id_factura == idFactura);
        }

        // ============================
        // Helpers privados
        // ============================

        private static void ValidarModelo(detalle_facturaDTO modelo)
        {
            if (string.IsNullOrWhiteSpace(modelo.descripcion))
                throw new ArgumentException("La descripción es requerida");

            if (modelo.cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero");

            if (modelo.precio_unitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor a cero");

            if (modelo.descuento < 0)
                throw new ArgumentException("El descuento no puede ser negativo");

            if (modelo.subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a cero");

            var subtotalCalculado = (modelo.cantidad * modelo.precio_unitario) - modelo.descuento;

            if (Math.Abs(subtotalCalculado - modelo.subtotal) > 0.01m)
                throw new ArgumentException(
                    $"El subtotal no coincide con el cálculo: {subtotalCalculado} vs {modelo.subtotal}"
                );
        }

        private async Task RecalcularTotalesFactura(int idFactura)
        {
            var subtotal = await CalcularSubtotalFactura(idFactura);

            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == idFactura);

            if (factura != null)
            {
                factura.subtotal = subtotal;
                factura.total = subtotal; // si hay impuestos, los sumas aquí
                factura.actualizado_en = DateTime.Now;

                _context.factura.Update(factura);
                await _context.SaveChangesAsync();
            }
        }
    }
}
