using AutoReparos.Cadastros.Application.Auth.Services.Interfaces;
using AutoReparos.Cadastros.Application.Shared.Interfaces;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;
using AutoReparos.Cadastros.Infrastructure.Data;
using AutoReparos.Cadastros.Infrastructure.Identity;
using AutoReparos.Cadastros.Infrastructure.Identity.Models;
using AutoReparos.Cadastros.Infrastructure.Identity.Services;
using AutoReparos.Cadastros.Infrastructure.Repositories;
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
            });

            services.AddIdentityCore<UsuarioIdentity>()
                .AddRoles<IdentityRole<Guid>>()
                .AddEntityFrameworkStores<AppDbContext>()
                .AddErrorDescriber<IdentityErrosTranslation>()
                .AddDefaultTokenProviders();

            services.AddScoped<IClienteRepository, ClienteRepository>();
            services.AddScoped<IVeiculoRepository, VeiculoRepository>();
            services.AddScoped<IUsuarioRepository, UsuarioRepository>();
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddSingleton<ChaveAssinaturaJwt>();
            services.AddScoped<IJwtService, JwtService>();

            return services;
        }
    }
}
