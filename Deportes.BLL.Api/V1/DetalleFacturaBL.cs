using AutoMapper;
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

        public async Task<List<detalle_facturaDTO>> ObtenerTodos()
        {
            var lista = await _context.detalle_factura
                         .OrderBy(d => d.id_detalle)
                         .ToListAsync();
            return _mapper.Map<List<detalle_facturaDTO>>(lista);
        }

        public async Task<detalle_facturaDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.detalle_factura
                            .Where(d => d.id_detalle == id)
                            .FirstOrDefaultAsync();
            return _mapper.Map<detalle_facturaDTO>(entidad);
        }

        public async Task<List<detalle_facturaDTO>> ObtenerPorFactura(int idFactura)
        {
            var lista = await _context.detalle_factura
                .Where(d => d.id_factura == idFactura)
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

            var nuevoDetalle = _mapper.Map<detalle_factura>(modelo);

            // Asignar fechas
            nuevoDetalle.creado_en = DateTime.Now;
            nuevoDetalle.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (modelo.cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero");

            if (modelo.precio_unitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor a cero");

            if (modelo.descuento < 0)
                throw new ArgumentException("El descuento no puede ser negativo");

            if (modelo.subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a cero");

            // Validar cálculo de subtotal
            var subtotalCalculado = (modelo.cantidad * modelo.precio_unitario) - modelo.descuento;
            if (Math.Abs(subtotalCalculado - modelo.subtotal) > 0.01m) // Tolerancia para decimales
                throw new ArgumentException($"El subtotal no coincide con el cálculo: {subtotalCalculado} vs {modelo.subtotal}");

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

            // Validar que la factura no esté anulada
            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == detalleExistente.id_factura);

            if (factura?.estado == 2)
                throw new InvalidOperationException("No se puede modificar un detalle de factura anulada");

            // Si cambia la factura, validar la nueva
            if (detalleExistente.id_factura != modelo.id_factura)
            {
                var nuevaFacturaExiste = await _context.factura
                    .AnyAsync(f => f.id_factura == modelo.id_factura && f.eliminado == 0);

                if (!nuevaFacturaExiste)
                    throw new ArgumentException($"Factura con ID {modelo.id_factura} no existe o está eliminada");
            }

            // Si tiene producto, validar que exista
            if (modelo.id_producto.HasValue)
            {
                var productoExiste = await _context.producto
                    .AnyAsync(p => p.id_producto == modelo.id_producto.Value);

                if (!productoExiste)
                    throw new ArgumentException($"Producto con ID {modelo.id_producto} no existe");
            }

            // Actualizar campos
            detalleExistente.id_factura = modelo.id_factura;
            detalleExistente.id_producto = modelo.id_producto;
            detalleExistente.descripcion = modelo.descripcion;
            detalleExistente.cantidad = modelo.cantidad;
            detalleExistente.precio_unitario = modelo.precio_unitario;
            detalleExistente.descuento = modelo.descuento;
            detalleExistente.subtotal = modelo.subtotal;
            detalleExistente.actualizado_en = DateTime.Now;

            // Validaciones
            if (modelo.cantidad <= 0)
                throw new ArgumentException("La cantidad debe ser mayor a cero");

            if (modelo.precio_unitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor a cero");

            if (modelo.descuento < 0)
                throw new ArgumentException("El descuento no puede ser negativo");

            if (modelo.subtotal <= 0)
                throw new ArgumentException("El subtotal debe ser mayor a cero");

            // Validar cálculo de subtotal
            var subtotalCalculado = (modelo.cantidad * modelo.precio_unitario) - modelo.descuento;
            if (Math.Abs(subtotalCalculado - modelo.subtotal) > 0.01m)
                throw new ArgumentException($"El subtotal no coincide con el cálculo: {subtotalCalculado} vs {modelo.subtotal}");

            _context.detalle_factura.Update(detalleExistente);
            await _context.SaveChangesAsync();

            // Recalcular totales de la factura
            await RecalcularTotalesFactura(modelo.id_factura);

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

            // Recalcular totales de la factura
            if (resultado > 0)
                await RecalcularTotalesFactura(idFactura);

            return resultado > 0;
        }

        public async Task<bool> EliminarPorFactura(int idFactura)
        {
            var detalles = await _context.detalle_factura
                .Where(d => d.id_factura == idFactura)
                .ToListAsync();

            if (detalles.Any())
            {
                _context.detalle_factura.RemoveRange(detalles);
                var resultado = await _context.SaveChangesAsync();
                return resultado > 0;
            }

            return true;
        }

        public async Task<decimal> CalcularSubtotalFactura(int idFactura)
        {
            var subtotal = await _context.detalle_factura
                .Where(d => d.id_factura == idFactura)
                .SumAsync(d => d.subtotal);

            return subtotal;
        }

        public async Task<bool> ValidarDetallesFactura(int idFactura)
        {
            var tieneDetalles = await _context.detalle_factura
                .AnyAsync(d => d.id_factura == idFactura);

            return tieneDetalles;
        }

        // MÉTODO PRIVADO PARA RECALCULAR TOTALES DE FACTURA
        private async Task RecalcularTotalesFactura(int idFactura)
        {
            var subtotal = await CalcularSubtotalFactura(idFactura);

            var factura = await _context.factura
                .FirstOrDefaultAsync(f => f.id_factura == idFactura);

            if (factura != null)
            {
                factura.subtotal = subtotal;
                factura.total = subtotal; // Aquí podrías agregar impuestos si los tienes
                factura.actualizado_en = DateTime.Now;

                _context.factura.Update(factura);
                await _context.SaveChangesAsync();
            }
        }
    }

    public interface IDetalleFacturaBL
    {
    }
}