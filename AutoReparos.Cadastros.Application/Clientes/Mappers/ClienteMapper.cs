using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Domain.Clientes.Entities;

namespace AutoReparos.Cadastros.Application.Clientes.Mappers
{
    public static class ClienteMapper
    {
        public static ClienteDto ToDto(Cliente cliente) => new(
            cliente.Id,
            cliente.Nome,
            cliente.Documento.Valor,
            cliente.Documento.Tipo.ToString(),
            cliente.Telefone,
            cliente.Email
        );
    }
}
