using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Deportes.DTO.Api;
using Deportes.DTO.Api.Models;


namespace Deportes.CSV.Api.V1
{
    [ServiceContract(Name = "DeportesProxyService")]
    public interface IDeportesProxyServiceV1
    {
        // Cliente
        [OperationContract(Name = "ObtenerClientes")]
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientes();

        [OperationContract(Name = "ObtenerClientePorId")]
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorId(int id);

        [OperationContract(Name = "ObtenerClientePorNit")]
        Task<ResponseDTO<ClienteDTO>> ObtenerClientePorNit(string nit);

        [OperationContract(Name = "CrearCliente")]
        Task<ResponseDTO<ClienteDTO>> CrearCliente(ClienteDTO cliente);

        [OperationContract(Name = "ActualizarCliente")]
        Task<ResponseDTO<ClienteDTO>> ActualizarCliente(ClienteDTO cliente);

        [OperationContract(Name = "EliminarCliente")]
        Task<ResponseDTO<bool>> EliminarCliente(int id);

        [OperationContract(Name = "ObtenerClientesActivos")]
        Task<ResponseDTO<List<ClienteDTO>>> ObtenerClientesActivos();

        [OperationContract(Name = "BuscarClientesPorNombre")]
        Task<ResponseDTO<List<ClienteDTO>>> BuscarClientesPorNombre(string nombre);

        [OperationContract(Name = "ActivarCliente")]
        Task<ResponseDTO<bool>> ActivarCliente(int id);

        [OperationContract(Name = "DesactivarCliente")]
        Task<ResponseDTO<bool>> DesactivarCliente(int id);

        // (Puede añadirse aquí más métodos que exponga el proxy, replicando los contratos del servicio)
    }
}