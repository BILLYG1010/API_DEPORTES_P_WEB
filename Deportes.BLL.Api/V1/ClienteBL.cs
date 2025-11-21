using AutoMapper;
using Deportes.CBL.Api.V1;
using Deportes.DAL.Api;
using Deportes.DAL.Api.Entities;
using Deportes.DTO.Api.Models;
using Microsoft.EntityFrameworkCore;


namespace Deportes.BLL.Api
{
    public class ClienteBL : IClienteBL
    {
        private readonly DeportesContext _context;
        private readonly IMapper _mapper;

        public ClienteBL(DeportesContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public async Task<List<ClienteDTO>> ObtenerTodos()
        {
            var lista = await _context.cliente
                         .OrderByDescending(x => x.creado_en)
                         .ToListAsync();
            return _mapper.Map<List<ClienteDTO>>(lista);
        }

        public async Task<ClienteDTO> ObtenerPorId(int id)
        {
            var entidad = await _context.cliente
                            .Where(x => x.id_cliente == id)
                            .FirstOrDefaultAsync();
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public async Task<ClienteDTO> ObtenerPorNit(string nit)
        {
            var entidad = await _context.cliente
                            .Where(x => x.nit == nit)
                            .FirstOrDefaultAsync();
            return _mapper.Map<ClienteDTO>(entidad);
        }

        public async Task<ClienteDTO> Crear(ClienteDTO modelo)
        {
            // Validar que el NIT no exista
            var nitExistente = await _context.cliente
                .AnyAsync(c => c.nit == modelo.nit);

            if (nitExistente)
                throw new ArgumentException($"Ya existe un cliente con el NIT {modelo.nit}");

            var nuevoCliente = _mapper.Map<cliente>(modelo);

            // Asignar fechas
            nuevoCliente.creado_en = DateTime.Now;
            nuevoCliente.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nit))
                throw new ArgumentException("El NIT es requerido");

            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre es requerido");

            _context.cliente.Add(nuevoCliente);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClienteDTO>(nuevoCliente);
        }

        public async Task<ClienteDTO> Actualizar(ClienteDTO modelo)
        {
            var clienteExistente = await _context.cliente
                .FirstOrDefaultAsync(x => x.id_cliente == modelo.id_cliente);

            if (clienteExistente == null)
                throw new ArgumentException($"Cliente con ID {modelo.id_cliente} no encontrado");

            // Validar que el NIT no esté duplicado (excluyendo el actual)
            var nitDuplicado = await _context.cliente
                .AnyAsync(c => c.nit == modelo.nit && c.id_cliente != modelo.id_cliente);

            if (nitDuplicado)
                throw new ArgumentException($"Ya existe otro cliente con el NIT {modelo.nit}");

            // Actualizar campos
            clienteExistente.nit = modelo.nit;
            clienteExistente.nombre = modelo.nombre;
            clienteExistente.direccion = modelo.direccion;
            clienteExistente.activo = modelo.activo;
            clienteExistente.actualizado_en = DateTime.Now;

            // Validar campos requeridos
            if (string.IsNullOrEmpty(modelo.nit))
                throw new ArgumentException("El NIT es requerido");

            if (string.IsNullOrEmpty(modelo.nombre))
                throw new ArgumentException("El nombre es requerido");

            _context.cliente.Update(clienteExistente);
            await _context.SaveChangesAsync();

            return _mapper.Map<ClienteDTO>(clienteExistente);
        }

        public async Task<bool> Eliminar(int id)
        {
            var cliente = await _context.cliente
                .FirstOrDefaultAsync(x => x.id_cliente == id);

            if (cliente == null)
                throw new ArgumentException($"Cliente con ID {id} no encontrado");

            // Verificar si tiene facturas como EMISOR
            var tieneFacturasComoEmisor = await _context.factura
                .AnyAsync(f => f.id_cliente_emisor == id && f.eliminado == 0);

            // Verificar si tiene facturas como RECEPTOR
            var tieneFacturasComoReceptor = await _context.factura
                .AnyAsync(f => f.id_cliente_receptor == id && f.eliminado == 0);

            if (tieneFacturasComoEmisor || tieneFacturasComoReceptor)
                throw new InvalidOperationException($"No se puede eliminar el cliente porque tiene facturas asociadas como emisor o receptor");

            _context.cliente.Remove(cliente);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<List<ClienteDTO>> ObtenerClientesActivos()
        {
            var lista = await _context.cliente
                .Where(x => x.activo == 1)
                .OrderBy(x => x.nombre)
                .ToListAsync();

            return _mapper.Map<List<ClienteDTO>>(lista);
        }

        public async Task<List<ClienteDTO>> BuscarPorNombre(string nombre)
        {
            var lista = await _context.cliente
                .Where(x => x.nombre.Contains(nombre))
                .OrderBy(x => x.nombre)
                .ToListAsync();

            return _mapper.Map<List<ClienteDTO>>(lista);
        }

        public async Task<bool> ActivarCliente(int id)
        {
            var cliente = await _context.cliente
                .FirstOrDefaultAsync(x => x.id_cliente == id);

            if (cliente == null)
                throw new ArgumentException($"Cliente con ID {id} no encontrado");

            cliente.activo = 1;
            cliente.actualizado_en = DateTime.Now;

            _context.cliente.Update(cliente);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        public async Task<bool> DesactivarCliente(int id)
        {
            var cliente = await _context.cliente
                .FirstOrDefaultAsync(x => x.id_cliente == id);

            if (cliente == null)
                throw new ArgumentException($"Cliente con ID {id} no encontrado");

            cliente.activo = 0;
            cliente.actualizado_en = DateTime.Now;

            _context.cliente.Update(cliente);
            var resultado = await _context.SaveChangesAsync();

            return resultado > 0;
        }

        // MÉTODOS ESPECÍFICOS PARA FACTURACIÓN

        public async Task<List<ClienteDTO>> ObtenerClientesParaFacturacion()
        {
            // Solo clientes activos que pueden ser usados en facturación
            var lista = await _context.cliente
                .Where(x => x.activo == 1)
                .OrderBy(x => x.nombre)
                .ToListAsync();

            return _mapper.Map<List<ClienteDTO>>(lista);
        }

        public async Task<List<FacturaDTO>> ObtenerFacturasComoEmisor(int idCliente)
        {
            var facturas = await _context.factura
                .Where(f => f.id_cliente_emisor == idCliente && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(facturas);
        }

        public async Task<List<FacturaDTO>> ObtenerFacturasComoReceptor(int idCliente)
        {
            var facturas = await _context.factura
                .Where(f => f.id_cliente_receptor == idCliente && f.eliminado == 0)
                .OrderByDescending(f => f.fecha_emision)
                .ToListAsync();

            return _mapper.Map<List<FacturaDTO>>(facturas);
        }

        public async Task<bool> ValidarClienteParaFacturacion(int idCliente)
        {
            var cliente = await _context.cliente
                .FirstOrDefaultAsync(x => x.id_cliente == idCliente);

            if (cliente == null)
                return false;

            // El cliente debe estar activo y tener NIT y nombre válidos
            return cliente.activo == 1 &&
                   !string.IsNullOrEmpty(cliente.nit) &&
                   !string.IsNullOrEmpty(cliente.nombre);
        }

        public async Task<Dictionary<string, object>> ObtenerEstadisticasCliente(int idCliente)
        {
            var estadisticas = new Dictionary<string, object>();

            var cliente = await _context.cliente.FindAsync(idCliente);
            if (cliente == null)
                throw new ArgumentException($"Cliente con ID {idCliente} no encontrado");

            estadisticas.Add("cliente", _mapper.Map<ClienteDTO>(cliente));

            // Facturas como emisor
            var facturasEmisor = await _context.factura
                .Where(f => f.id_cliente_emisor == idCliente && f.eliminado == 0)
                .ToListAsync();
            estadisticas.Add("total_facturas_emisor", facturasEmisor.Count);
            estadisticas.Add("monto_total_emisor", facturasEmisor.Sum(f => f.total));

            // Facturas como receptor
            var facturasReceptor = await _context.factura
                .Where(f => f.id_cliente_receptor == idCliente && f.eliminado == 0)
                .ToListAsync();
            estadisticas.Add("total_facturas_receptor", facturasReceptor.Count);
            estadisticas.Add("monto_total_receptor", facturasReceptor.Sum(f => f.total));

            return estadisticas;
        }
    }

   
}