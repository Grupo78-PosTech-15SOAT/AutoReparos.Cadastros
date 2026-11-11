using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Application.Dashboard.Services;
using AutoReparos.Cadastros.Application.OrdensServicos.Services.Interfaces;
using AutoReparos.Cadastros.Application.Shared.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Domain.OrdensServicos.Repositories;
using AutoReparos.Cadastros.Domain.Servicos.Repositories;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;
using AutoReparos.Cadastros.Infrastructure.Data;
using AutoReparos.Cadastros.Infrastructure.Data.Interceptors;
using AutoReparos.Cadastros.Infrastructure.Identity;
using AutoReparos.Cadastros.Infrastructure.Identity.Models;
using AutoReparos.Cadastros.Infrastructure.Identity.Services;
using AutoReparos.Cadastros.Infrastructure.Repositories;
using AutoReparos.Cadastros.Infrastructure.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AutoReparos.Cadastros.Infrastructure.IoC
{
    public static class DependencyInjectionInfra
    {
        /// <summary>
        /// Método de extensão para registrar serviços relacionados à camada de Infraestructure
        /// </summary>
        /// <param name="services">Collection de serviços da aplicação</param>
        /// <returns>Collection de services com os serviços de Infraestructure registrados</returns>
        public static IServiceCollection AddInfrastructure(this IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>((serviceProvider, options) =>
            {
                var config = serviceProvider.GetRequiredService<IConfiguration>();
                options.UseNpgsql(config.GetConnectionString("DbConnection"),
                    b => b.MigrationsAssembly(typeof(AppDbContext).Assembly.FullName));
                options.AddInterceptors(new OrdemServicoMetricsInterceptor());
            });

            services.AddIdentityCore<UsuarioIdentity>()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddErrorDescriber<IdentityErrosTranslation>()
                .AddDefaultTokenProviders();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();
            services.AddScoped<IServicoRepository, ServicoRepository>();
            services.AddScoped<IInsumoRepository, InsumoRepository>();
            services.AddScoped<IOrdemServicoRepository, OrdemServicoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IJwtService, JwtService>();
            services.AddScoped<IAprovacaoTokenService, AprovacaoTokenService>();
            services.AddScoped<INotificacaoService, NotificacaoService>();
            services.AddScoped<IDashboardQueryService, DashboardQueryService>();

            return services;
        }
    }
}
