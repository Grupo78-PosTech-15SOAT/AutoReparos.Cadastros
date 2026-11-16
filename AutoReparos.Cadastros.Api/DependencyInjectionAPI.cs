using AutoReparos.Cadastros.Api.Controllers;
using AutoReparos.Cadastros.Api.Handlers;
using AutoReparos.Cadastros.Api.Security;
using AutoReparos.Cadastros.Api.Services;
using AutoReparos.Cadastros.Application.Shared.Interfaces;
using AutoReparos.Cadastros.Infrastructure.Identity.Services;
using AutoReparos.Cadastros.Infrastructure.Settings;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace AutoReparos.Cadastros.Api
{
    public static class DependencyInjectionApi
    {
        /// <summary>
        /// Método de extensão para registrar serviços relacionados à camada de API
        /// </summary>
        /// <param name="services">Collection de serviços da aplicação</param>
        /// <param name="configuration">Configurações da aplicação</param>
        /// <returns>Collection de services com os serviços da API registrados</returns>
        public static IServiceCollection AddAPI(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection(JwtSettings.SectionName));
            services.Configure<SeedUsuarioSettings>(configuration.GetSection(SeedUsuarioSettings.SectionName));

            services.Configure<InternoSettings>(configuration.GetSection(InternoSettings.SectionName));

            services.AddExceptionHandler<GlobalExceptionHandler>();
            services.AddProblemDetails();

            services.AddCors(options =>
            {
                options.AddPolicy("AllowFrontend", policy =>
                {
                    policy.SetIsOriginAllowed(_ => true)
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials()
                          .WithExposedHeaders("X-Correlation-Id");
                });
            });

            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer()
            .AddScheme<AuthenticationSchemeOptions, ChaveInternaAuthenticationHandler>(ChaveInternaAuthenticationHandler.Scheme, null);

            // Valida os tokens do próprio Cadastros pela chave pública (sem segredo compartilhado)
            services.AddOptions<JwtBearerOptions>(JwtBearerDefaults.AuthenticationScheme)
                .Configure<ChaveAssinaturaJwt>((o, chave) =>
                {
                    o.RequireHttpsMetadata = false;
                    o.SaveToken = true;
                    o.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = chave.ChavePublica,
                        ValidAlgorithms = [SecurityAlgorithms.RsaSha256],
                        ValidateIssuer = true,
                        ValidIssuer = chave.Issuer,
                        ValidateAudience = false
                    };
                });

            services.AddAuthorization(options =>
            {
                // Operadores da aplicação (oficina) têm acesso amplo às rotas operacionais
                options.AddPolicy("OperadorOficina", policy =>
                    policy.RequireRole("Administrador", "Atendente", "Mecanico"));

                // Clientes autenticados via token efêmero da Lambda têm acesso às rotas restritas do portal
                options.AddPolicy("ClientePolicy", policy =>
                    policy.RequireRole("Cliente"));

                // Consulta de cliente e veículo: usuário autenticado (JWT repassado pelo OS) ou, sem usuário, a chave interna
                options.AddPolicy("ConsultaCadastro", policy => policy
                    .AddAuthenticationSchemes(JwtBearerDefaults.AuthenticationScheme, ChaveInternaAuthenticationHandler.Scheme)
                    .RequireRole("Administrador", "Atendente", "Mecanico", ChaveInternaAuthenticationHandler.RoleServicoInterno));

                // Rotas /internal: somente com X-Chave-Interna válido
                options.AddPolicy("ChaveInterna", policy => policy
                    .AddAuthenticationSchemes(ChaveInternaAuthenticationHandler.Scheme)
                    .RequireRole(ChaveInternaAuthenticationHandler.RoleServicoInterno));
            });

            services.AddScoped<AuthController>();
            services.AddScoped<UsuarioController>();
            services.AddScoped<ClienteController>();
            services.AddScoped<VeiculoController>();
            services.AddScoped<PortalClienteController>();

            services.AddHttpContextAccessor();
            services.AddScoped<ICurrentUserService, CurrentUserService>();

            return services;
        }
    }
}
