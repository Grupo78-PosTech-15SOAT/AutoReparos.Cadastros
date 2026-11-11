using AutoReparos.Cadastros.Application.Auth.UseCases;
using AutoReparos.Cadastros.Application.Auth.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Clientes.UseCases;
using AutoReparos.Cadastros.Application.Clientes.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.Insumos.UseCases;
using AutoReparos.Cadastros.Application.Insumos.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Interfaces;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Aprovacao;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Aprovacao.Interfaces;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Core.Interfaces;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo;
using AutoReparos.Cadastros.Application.OrdensServicos.UseCases.Fluxo.Interfaces;
using AutoReparos.Cadastros.Application.Servicos.UseCases;
using AutoReparos.Cadastros.Application.Servicos.UseCases.Interfaces;
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

            // Veículos
            services.AddScoped<ICriarVeiculoUseCase, CriarVeiculoUseCase>();
            services.AddScoped<IListarVeiculosUseCase, ListarVeiculosUseCase>();
            services.AddScoped<IObterVeiculoPorIdUseCase, ObterVeiculoPorIdUseCase>();
            services.AddScoped<IObterVeiculoPorPlacaUseCase, ObterVeiculoPorPlacaUseCase>();
            services.AddScoped<IAtualizarVeiculoUseCase, AtualizarVeiculoUseCase>();
            services.AddScoped<IExcluirVeiculoUseCase, ExcluirVeiculoUseCase>();

            // Serviços
            services.AddScoped<ICriarServicoUseCase, CriarServicoUseCase>();
            services.AddScoped<IObterServicoPorIdUseCase, ObterServicoPorIdUseCase>();
            services.AddScoped<IListarServicosUseCase, ListarServicosUseCase>();
            services.AddScoped<IObterTempoMedioServicosUseCase, ObterTempoMedioServicosUseCase>();
            services.AddScoped<IObterTempoMedioServicoPorIdUseCase, ObterTempoMedioServicoPorIdUseCase>();
            services.AddScoped<IAtualizarServicoUseCase, AtualizarServicoUseCase>();
            services.AddScoped<IExcluirServicoUseCase, ExcluirServicoUseCase>();

            // Insumos
            services.AddScoped<ICriarInsumoUseCase, CriarInsumoUseCase>();
            services.AddScoped<IObterInsumoPorIdUseCase, ObterInsumoPorIdUseCase>();
            services.AddScoped<IListarInsumosUseCase, ListarInsumosUseCase>();
            services.AddScoped<IAtualizarInsumoUseCase, AtualizarInsumoUseCase>();
            services.AddScoped<IAdicionarEstoqueUseCase, AdicionarEstoqueUseCase>();
            services.AddScoped<IRemoverEstoqueUseCase, RemoverEstoqueUseCase>();
            services.AddScoped<IExcluirInsumoUseCase, ExcluirInsumoUseCase>();

            // Ordens de Serviço - Kanban Strategies
            services.AddScoped<AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces.IKanbanCardStrategy, AutoReparos.Cadastros.Application.OrdensServicos.Strategies.ReceivedKanbanCardStrategy>();
            services.AddScoped<AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces.IKanbanCardStrategy, AutoReparos.Cadastros.Application.OrdensServicos.Strategies.DiagnosisKanbanCardStrategy>();
            services.AddScoped<AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces.IKanbanCardStrategy, AutoReparos.Cadastros.Application.OrdensServicos.Strategies.ApprovalKanbanCardStrategy>();
            services.AddScoped<AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces.IKanbanCardStrategy, AutoReparos.Cadastros.Application.OrdensServicos.Strategies.ExecutionKanbanCardStrategy>();
            services.AddScoped<AutoReparos.Cadastros.Application.OrdensServicos.Strategies.Interfaces.IKanbanCardStrategy, AutoReparos.Cadastros.Application.OrdensServicos.Strategies.FinishedKanbanCardStrategy>();

            // Ordens de Serviço
            services.AddScoped<ICriarOrdemServicoUseCase, CriarOrdemServicoUseCase>();
            services.AddScoped<IListarOrdensServicoUseCase, ListarOrdensServicoUseCase>();
            services.AddScoped<IListarKanbanOrdensServicoUseCase, ListarKanbanOrdensServicoUseCase>();
            services.AddScoped<IListarFilaOrdensServicoUseCase, ListarFilaOrdensServicoUseCase>();
            services.AddScoped<IObterOrdemServicoPorIdUseCase, ObterOrdemServicoPorIdUseCase>();
            services.AddScoped<IObterOrdemServicoPublicaPorIdUseCase, ObterOrdemServicoPublicaPorIdUseCase>();
            services.AddScoped<IConsultarOrdensServicoPorDocumentoOuPlacaUseCase, ConsultarOrdensServicoPorDocumentoOuPlacaUseCase>();
            services.AddScoped<IAdicionarServicoOrdemServicoUseCase, AdicionarServicoOrdemServicoUseCase>();
            services.AddScoped<IAdicionarInsumoOrdemServicoUseCase, AdicionarInsumoOrdemServicoUseCase>();
            services.AddScoped<IIniciarDiagnosticoOrdemServicoUseCase, IniciarDiagnosticoOrdemServicoUseCase>();
            services.AddScoped<IEnviarOrdemServicoParaAprovacaoUseCase, EnviarOrdemServicoParaAprovacaoUseCase>();
            services.AddScoped<IAprovarOrdemServicoUseCase, AprovarOrdemServicoUseCase>();
            services.AddScoped<IRecusarOrdemServicoUseCase, RecusarOrdemServicoUseCase>();
            services.AddScoped<IIniciarServicoOrdemServicoUseCase, IniciarServicoOrdemServicoUseCase>();
            services.AddScoped<IConcluirServicoOrdemServicoUseCase, ConcluirServicoOrdemServicoUseCase>();
            services.AddScoped<IEntregarOrdemServicoUseCase, EntregarOrdemServicoUseCase>();
            services.AddScoped<IObterMinhasOrdensServicoUseCase, ObterMinhasOrdensServicoUseCase>();

            return services;
        }
    }
}
