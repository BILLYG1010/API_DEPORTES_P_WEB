using Deportes.CBL.Api.V1;
using Deportes.CSV.Api.V1;
using Deportes.DTO.Api;
using Deportes.DTO.Api.Models;

namespace Deportes.BSV.Api.V1
{
    public class DeportesServiceV1 :
        IDeportesServiceV1,
        IDeportesProxyServiceV1   // 👈 implementa también el proxy SOAP
    {
        private readonly IClienteBL _clienteBL;
        private readonly IProductoBL _productoBL;
        private readonly IFacturaBL _facturaBL;
        private readonly IDetalleFacturaBL _detalleBL;
        private readonly IBitacoraCertificacionBL _bitacoraBL;

        public DeportesServiceV1(
            IClienteBL clienteBL,
            IProductoBL productoBL,
            IFacturaBL facturaBL,
            IDetalleFacturaBL detalleBL,
            IBitacoraCertificacionBL bitacoraBL
        )
        {
            _clienteBL = clienteBL;
            _productoBL = productoBL;
            _facturaBL = facturaBL;
            _detalleBL = detalleBL;
            _bitacoraBL = bitacoraBL;
        }

        // ===== CLIENTE =====
        public async Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientes()
        {
            try
            {
                var data = await _clienteBL.ObtenerTodos();
                return ResponseDTO<List<ClienteDTO>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ResponseDTO<List<ClienteDTO>>.Fail(ex.Message);
            }
        }

        public async Task<ResponseDTO<ClienteDTO>> ObtenerClientePorId(int id)
        {
            try
            {
                var data = await _clienteBL.ObtenerPorId(id);
                return ResponseDTO<ClienteDTO>.Ok(data);
            }
            catch (Exception ex)
            {
                return ResponseDTO<ClienteDTO>.Fail(ex.Message);
            }
        }

        public async Task<ResponseDTO<ClienteDTO>> CrearCliente(ClienteDTO cliente)
        {
            try
            {
                var data = await _clienteBL.Crear(cliente);
                return ResponseDTO<ClienteDTO>.Ok(data);
            }
            catch (Exception ex)
            {
                return ResponseDTO<ClienteDTO>.Fail(ex.Message);
            }
        }

        public async Task<ResponseDTO<ClienteDTO>> EditarCliente(ClienteDTO cliente)
        {
            try
            {
                var data = await _clienteBL.Actualizar(cliente);
                return ResponseDTO<ClienteDTO>.Ok(data);
            }
            catch (Exception ex)
            {
                return ResponseDTO<ClienteDTO>.Fail(ex.Message);
            }
        }

        public async Task<ResponseDTO<bool>> EliminarCliente(int id)
        {
            try
            {
                var ok = await _clienteBL.Eliminar(id);
                return ResponseDTO<bool>.Ok(ok);
            }
            catch (Exception ex)
            {
                return ResponseDTO<bool>.Fail(ex.Message);
            }
        }

        // ===== PRODUCTO ===== (patrón igual)
        public async Task<ResponseDTO<List<ProductoDTO>>> ObtenerProductos()
        {
            try { return ResponseDTO<List<ProductoDTO>>.Ok(await _productoBL.ObtenerTodos()); }
            catch (Exception ex) { return ResponseDTO<List<ProductoDTO>>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<ProductoDTO>> ObtenerProductoPorId(int id)
        {
            try { return ResponseDTO<ProductoDTO>.Ok(await _productoBL.ObtenerPorId(id)); }
            catch (Exception ex) { return ResponseDTO<ProductoDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<ProductoDTO>> CrearProducto(ProductoDTO producto)
        {
            try { return ResponseDTO<ProductoDTO>.Ok(await _productoBL.Crear(producto)); }
            catch (Exception ex) { return ResponseDTO<ProductoDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<ProductoDTO>> EditarProducto(ProductoDTO producto)
        {
            try { return ResponseDTO<ProductoDTO>.Ok(await _productoBL.Actualizar(producto)); }
            catch (Exception ex) { return ResponseDTO<ProductoDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<bool>> EliminarProducto(int id)
        {
            try { return ResponseDTO<bool>.Ok(await _productoBL.Eliminar(id)); }
            catch (Exception ex) { return ResponseDTO<bool>.Fail(ex.Message); }
        }

        // ===== FACTURA / DETALLE / BITACORA =====
        // Sigue el mismo patrón llamando a tus BL ya corregidos
        public async Task<ResponseDTO<List<FacturaDTO>>> ObtenerFacturas()
        {
            try { return ResponseDTO<List<FacturaDTO>>.Ok(await _facturaBL.ObtenerTodos()); }
            catch (Exception ex) { return ResponseDTO<List<FacturaDTO>>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<FacturaDTO>> ObtenerFacturaPorId(int id)
        {
            try { return ResponseDTO<FacturaDTO>.Ok(await _facturaBL.ObtenerPorId(id)); }
            catch (Exception ex) { return ResponseDTO<FacturaDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<FacturaDTO>> CrearFactura(FacturaDTO factura)
        {
            try { return ResponseDTO<FacturaDTO>.Ok(await _facturaBL.Crear(factura)); }
            catch (Exception ex) { return ResponseDTO<FacturaDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<FacturaDTO>> EditarFactura(FacturaDTO factura)
        {
            try { return ResponseDTO<FacturaDTO>.Ok(await _facturaBL.Actualizar(factura)); }
            catch (Exception ex) { return ResponseDTO<FacturaDTO>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<bool>> EliminarFactura(int id)
        {
            try { return ResponseDTO<bool>.Ok(await _facturaBL.Eliminar(id)); }
            catch (Exception ex) { return ResponseDTO<bool>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<List<detalle_facturaDTO>>> ObtenerDetallesPorFactura(int idFactura)
        {
            try { return ResponseDTO<List<detalle_facturaDTO>>.Ok(await _detalleBL.ObtenerPorFactura(idFactura)); }
            catch (Exception ex) { return ResponseDTO<List<detalle_facturaDTO>>.Fail(ex.Message); }
        }

        public async Task<ResponseDTO<List<bitacora_certificacionDTO>>> ObtenerBitacorasPorFactura(int idFactura)
        {
            try { return ResponseDTO<List<bitacora_certificacionDTO>>.Ok(await _bitacoraBL.ObtenerPorFactura(idFactura)); }
            catch (Exception ex) { return ResponseDTO<List<bitacora_certificacionDTO>>.Fail(ex.Message); }
        }
    }
}
