using AutoReparos.Cadastros.Application.Auth.UseCases;
using AutoReparos.Cadastros.Application.Auth.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Clientes.UseCases;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Usuarios.UseCases;
using AutoReparos.Cadastros.Application.Usuarios.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Veiculos.UseCases;
using AutoReparos.Cadastros.Application.Veiculos.UseCases.Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace AutoReparos.Cadastros.Application
{
    public static class DependencyInjection
    {
        /// <summary>
        /// Método de extensão para registrar os casos de uso da camada de Application
        /// </summary>
        /// <param name="services">Collection de serviços da aplicação</param>
        /// <returns>Collection de services com os casos de uso de Application registrados</returns>
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            // Auth
            services.AddScoped<ILoginUseCase, LoginUseCase>();

            // Usuários
            services.AddScoped<ICriarUsuarioUseCase, CriarUsuarioUseCase>();
            services.AddScoped<IObterUsuarioPorIdUseCase, ObterUsuarioPorIdUseCase>();
            services.AddScoped<IListarUsuariosUseCase, ListarUsuariosUseCase>();
            services.AddScoped<IAtualizarUsuarioUseCase, AtualizarUsuarioUseCase>();
            services.AddScoped<IExcluirUsuarioUseCase, ExcluirUsuarioUseCase>();

            // Clientes
            services.AddScoped<ICriarClienteUseCase, CriarClienteUseCase>();
            services.AddScoped<IObterClientePorIdUseCase, ObterClientePorIdUseCase>();
            services.AddScoped<IListarClientesUseCase, ListarClientesUseCase>();
            services.AddScoped<IAtualizarClienteUseCase, AtualizarClienteUseCase>();
            services.AddScoped<IExcluirClienteUseCase, ExcluirClienteUseCase>();
            services.AddScoped<IObterMeusVeiculosUseCase, ObterMeusVeiculosUseCase>();
            services.AddScoped<IAutenticarClienteUseCase, AutenticarClienteUseCase>();

            // Veículos
            services.AddScoped<ICriarVeiculoUseCase, CriarVeiculoUseCase>();
            services.AddScoped<IListarVeiculosUseCase, ListarVeiculosUseCase>();
            services.AddScoped<IObterVeiculoPorIdUseCase, ObterVeiculoPorIdUseCase>();
            services.AddScoped<IObterVeiculoPorPlacaUseCase, ObterVeiculoPorPlacaUseCase>();
            services.AddScoped<IAtualizarVeiculoUseCase, AtualizarVeiculoUseCase>();
            services.AddScoped<IExcluirVeiculoUseCase, ExcluirVeiculoUseCase>();

            return services;
        }
    }
}
