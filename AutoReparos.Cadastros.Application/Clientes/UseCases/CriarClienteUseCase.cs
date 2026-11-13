using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Clientes.Mappers;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Clientes.Exceptions;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Clientes.ValueObjects;
using AutoReparos.Cadastros.Domain.Shared.Exceptions;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class CriarClienteUseCase(IClienteRepository repository) : ICriarClienteUseCase
    {
        public async Task<ClienteDto> ExecuteAsync(ClienteCreateDto dto)
        {
            var normalizedDocumento = Documento.Normalizar(dto.Documento);
            var normalizedEmail = dto.Email.Trim().ToLower();

            var clienteExiste = await repository.GetByDocumentoOrEmail(normalizedDocumento, normalizedEmail);
            if (clienteExiste != null)
            {
                if (clienteExiste.Documento.Valor == normalizedDocumento)
                {
                    throw new InvalidDocumentoException("CPF ou CNPJ já cadastrado.");
                }
                throw new InvalidEmailException("E-mail já cadastrado.");
            }

            var cliente = new Cliente(dto.Nome, dto.Documento, dto.Telefone, dto.Email);

            await repository.Create(cliente);
            return ClienteMapper.ToDto(cliente);
        }
    }
}
