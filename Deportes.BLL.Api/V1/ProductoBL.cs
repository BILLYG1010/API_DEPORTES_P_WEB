using AutoMapper;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deportes.BLL.Api
{
    public class ProductoBL : IProductoBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public ProductoBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ProductoDTO>> ObtenerTodos()
        {
            var lista = await _context.producto
                         .Where(p => p.activo == 1)
                         .OrderBy(p => p.nombre)
                         .ToListAsync();
            return _mapper.Map<List<ProductoDTO>>(lista);
        }

        public async Task<ProductoDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.producto
                            .Where(p => p.id_producto == id && p.activo == 1)
                            .FirstOrDefaultAsync();
            return _mapper.Map<ProductoDTO>(entidad);
        }

        public async Task<ProductoDTO> ObtenerPorSku(string sku)
        {
            var entidad = await _context.producto
                            .Where(p => p.sku == sku && p.activo == 1)
                            .FirstOrDefaultAsync();
            return _mapper.Map<ProductoDTO>(entidad);
        }

        public async Task<ProductoDTO> Crear(ProductoDTO modelo)
        {
            // Validar que el SKU no exista
            var skuExistente = await _context.producto
                .AnyAsync(p => p.sku == modelo.sku && p.activo == 1);

            if (skuExistente)
                throw new ArgumentException($"Ya existe un producto con el SKU {modelo.sku}");

            var nuevoProducto = _mapper.Map<producto>(modelo);

            // Asignar valores por defecto
            nuevoProducto.creado_en = DateTime.Now;
            nuevoProducto.actualizado_en = DateTime.Now;
            nuevoProducto.activo = 1;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.sku))
                throw new ArgumentException("El SKU es requerido");

            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre es requerido");

            if (modelo.precio_unitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor a cero");

            if (modelo.cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa");

            _context.producto.Add(nuevoProducto);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductoDTO>(nuevoProducto);
        }

        public async Task<ProductoDTO> Actualizar(ProductoDTO modelo)
        {
            var productoExistente = await _context.producto
                .FirstOrDefaultAsync(p => p.id_producto == modelo.id_producto && p.activo == 1);

            if (productoExistente == null)
                throw new ArgumentException($"Producto con ID {modelo.id_producto} no encontrado");

            // Validar que el SKU no esté duplicado
            var skuDuplicado = await _context.producto
                .AnyAsync(p => p.sku == modelo.sku && p.id_producto != modelo.id_producto && p.activo == 1);

            if (skuDuplicado)
                throw new ArgumentException($"Ya existe otro producto con el SKU {modelo.sku}");

            // Actualizar campos
            productoExistente.sku = modelo.sku;
            productoExistente.nombre = modelo.nombre;
            productoExistente.descripcion = modelo.descripcion;
            productoExistente.precio_unitario = modelo.precio_unitario;
            productoExistente.cantidad = modelo.cantidad;
            productoExistente.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.sku))
                throw new ArgumentException("El SKU es requerido");

            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre es requerido");

            if (modelo.precio_unitario <= 0)
                throw new ArgumentException("El precio unitario debe ser mayor a cero");

            if (modelo.cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa");

            _context.producto.Update(productoExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<ProductoDTO>(productoExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var producto = await _context.producto
                .FirstOrDefaultAsync(p => p.id_producto == id && p.activo == 1);

            if (producto == null)
                throw new ArgumentException($"Producto con ID {id} no encontrado");

            // Verificar si está en detalles de factura
            var enDetallesFactura = await _context.detalle_factura
                .AnyAsync(d => d.id_producto == id);

            if (enDetallesFactura)
                throw new InvalidOperationException("No se puede eliminar el producto porque está en detalles de factura");

            // Eliminación lógica
            producto.activo = 0;
            producto.actualizado_en = DateTime.Now;

            _context.producto.Update(producto);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<List<ProductoDTO>> ObtenerProductosActivos()
        {
            var lista = await _context.producto
                .Where(p => p.activo == 1)
                .OrderBy(p => p.nombre)
                .ToListAsync();

            return _mapper.Map<List<ProductoDTO>>(lista);
        }

        public async Task<List<ProductoDTO>> BuscarPorNombre(string nombre)
        {
            var lista = await _context.producto
                .Where(p => p.nombre.Contains(nombre) && p.activo == 1)
                .OrderBy(p => p.nombre)
                .ToListAsync();

            return _mapper.Map<List<ProductoDTO>>(lista);
        }

        public async Task<bool> ActualizarStock(int id, int cantidad)
        {
            var producto = await _context.producto
                .FirstOrDefaultAsync(p => p.id_producto == id && p.activo == 1);

            if (producto == null)
                throw new ArgumentException($"Producto con ID {id} no encontrado");

            if (cantidad < 0)
                throw new ArgumentException("La cantidad no puede ser negativa");

            producto.cantidad = cantidad;
            producto.actualizado_en = DateTime.Now;

            _context.producto.Update(producto);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<bool> ActualizarPrecio(int id, decimal precio)
        {
            var producto = await _context.producto
                .FirstOrDefaultAsync(p => p.id_producto == id && p.activo == 1);

            if (producto == null)
                throw new ArgumentException($"Producto con ID {id} no encontrado");

            if (precio <= 0)
                throw new ArgumentException("El precio debe ser mayor a cero");

            producto.precio_unitario = precio;
            producto.actualizado_en = DateTime.Now;

            _context.producto.Update(producto);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }
    }

    public interface IProductoBL
    {
    }
}