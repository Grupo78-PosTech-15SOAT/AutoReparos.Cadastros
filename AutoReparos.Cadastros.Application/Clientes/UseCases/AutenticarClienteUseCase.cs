using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Request;
using AutoReparos.Cadastros.Application.Clientes.DTOs.Response;
using AutoReparos.Cadastros.Application.Clientes.Mappers;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;

namespace AutoReparos.Cadastros.Application.Clientes.UseCases
{
    public class AutenticarClienteUseCase(IClienteRepository repository, IJwtService jwtService) : IAutenticarClienteUseCase
    {
        public async Task<ResultadoAutenticacaoCliente> ExecuteAsync(AutenticarClienteDto dto)
        {
            var documento = new string((dto.Cpf ?? string.Empty).Where(char.IsDigit).ToArray());
            if (documento.Length == 0 || string.IsNullOrWhiteSpace(dto.Email))
                return new ResultadoAutenticacaoCliente(StatusAutenticacaoCliente.NaoEncontrado);

            var cliente = await repository.GetByDocumento(documento);
            if (cliente is null || !string.Equals(cliente.Email.Endereco, dto.Email.Trim(), StringComparison.OrdinalIgnoreCase))
                return new ResultadoAutenticacaoCliente(StatusAutenticacaoCliente.NaoEncontrado);

            if (!cliente.Ativo)
                return new ResultadoAutenticacaoCliente(StatusAutenticacaoCliente.Inativo);

            return new ResultadoAutenticacaoCliente(StatusAutenticacaoCliente.Autenticado,
                new ClienteAutenticadoDto(ClienteMapper.ToDto(cliente), jwtService.GenerateToken(cliente)));
        }
    }
}
