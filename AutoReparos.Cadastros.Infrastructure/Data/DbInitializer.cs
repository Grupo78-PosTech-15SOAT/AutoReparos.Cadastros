using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Clientes.Repositories;
using AutoReparos.Cadastros.Domain.Usuarios.Entities;
using AutoReparos.Cadastros.Domain.Usuarios.Enums;
using AutoReparos.Cadastros.Domain.Usuarios.Repositories;
using AutoReparos.Cadastros.Domain.Veiculos.Entities;
using AutoReparos.Cadastros.Domain.Veiculos.Repositories;
using AutoReparos.Cadastros.Domain.Veiculos.ValueObjects;
using AutoReparos.Cadastros.Infrastructure.Settings;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoReparos.Cadastros.Infrastructure.Data
{
    public static class DbInitializer
    {
        public static async Task SeedDataAsync(IServiceProvider serviceProvider, bool skipMigration = false)
        {
            using var scope = serviceProvider.CreateScope();
            var services = scope.ServiceProvider;
            try
            {
                var context = services.GetRequiredService<AppDbContext>();
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DbInitializer");

                if (!skipMigration)
                {
                    logger.LogInformation("Iniciando migração do banco de dados...");
                    await context.Database.MigrateAsync();
                    logger.LogInformation("Migração concluída.");
                }

                logger.LogInformation("Iniciando Seed...");

                var usuarioRepository = services.GetRequiredService<IUsuarioRepository>();
                var clienteRepository = services.GetRequiredService<IClienteRepository>();
                var veiculoRepository = services.GetRequiredService<IVeiculoRepository>();
                var seedSettings = services.GetRequiredService<IOptions<SeedUsuarioSettings>>().Value;
                var environment = services.GetRequiredService<IHostEnvironment>();

                await SeedUsuariosAsync(usuarioRepository, seedSettings, logger);

                if (environment.IsDevelopment() || environment.IsEnvironment("Testing") || environment.IsStaging())
                {
                    var clientes = await SeedClientesAsync(clienteRepository, logger);
                    var veiculos = await SeedVeiculosAsync(veiculoRepository, clientes, logger);
                }
            }
            catch (Exception ex)
            {
                var loggerFactory = services.GetRequiredService<ILoggerFactory>();
                var logger = loggerFactory.CreateLogger("DbInitializer");
                logger.LogError(ex, "Ocorreu um erro ao inicializar ou popular o banco de dados.");
                throw new InvalidOperationException("Erro durante a inicialização e seed do banco de dados.", ex);
            }
        }

        private static async Task SeedUsuariosAsync(
            IUsuarioRepository usuarioRepository,
            SeedUsuarioSettings settings,
            ILogger logger)
        {
            if (string.IsNullOrEmpty(settings.Email) || string.IsNullOrEmpty(settings.Password))
            {
                logger.LogWarning("Configurações de Seed de usuário não encontradas ou incompletas.");
                return;
            }

            var existingAdmin = await usuarioRepository.GetByEmailAsync(settings.Email);
            if (existingAdmin is not null) return;

            logger.LogInformation("Criando usuário administrador padrão: {Email}", settings.Email);

            var adminUser = new Usuario("Administrador do Sistema", settings.Email, ETipoUsuario.Administrador);
            await usuarioRepository.CreateAsync(adminUser, settings.Password);

            logger.LogInformation("Usuário administrador criado com sucesso.");
        }

        private static async Task<List<Cliente>> SeedClientesAsync(
            IClienteRepository clienteRepository,
            ILogger logger)
        {
            var (_, total) = await clienteRepository.GetAll(null, 0, 1);
            if (total > 0)
            {
                logger.LogInformation("Clientes já existem, pulando seed.");
                var (existentes, _) = await clienteRepository.GetAll(null, 0, 3);
                return existentes.ToList();
            }

            logger.LogInformation("Criando clientes de desenvolvimento...");

            var clientes = new List<Cliente>
            {
                new("Leandro Tavares", "97632180044", "27999111111", "leandro.silva@email.com"),
                new("Fernanda Oliveira Costa", "48743358020", "27999222222", "fernanda.costa@email.com"),
                new("Roberto Alves Pereira", "60729292061", "27999333333", "roberto.pereira@email.com"),
            };

            foreach (var cliente in clientes)
                await clienteRepository.Create(cliente);

            logger.LogInformation("{Count} clientes criados.", clientes.Count);
            return clientes;
        }

        private static async Task<List<Veiculo>> SeedVeiculosAsync(
            IVeiculoRepository veiculoRepository,
            List<Cliente> clientes,
            ILogger logger)
        {
            var (_, total) = await veiculoRepository.GetAll(null, 0, 1);
            if (total > 0)
            {
                logger.LogInformation("Veículos já existem, pulando seed.");
                var (existentes, _) = await veiculoRepository.GetAll(null, 0, 3);
                return existentes.ToList();
            }

            logger.LogInformation("Criando veículos de desenvolvimento...");

            var veiculos = new List<Veiculo>
            {
                new(clientes[0].Id, "Volkswagen", "Gol", 2019, 2020,
                    new Placa("ABC1D23"),
                    new Chassi("9BWZZZ377VT004251"),
                    new Renavam("12345678901")),

                new(clientes[1].Id, "Chevrolet", "Onix", 2021, 2021,
                    new Placa("XYZ2E34"),
                    new Chassi("9BWZZZ377VT004252"),
                    new Renavam("23456789012")),

                new(clientes[2].Id, "Fiat", "Strada", 2020, 2021,
                    new Placa("DEF3F45"),
                    new Chassi("9BWZZZ377VT004253"),
                    new Renavam("34567890123")),
            };

            foreach (var veiculo in veiculos)
                await veiculoRepository.Create(veiculo);

            logger.LogInformation("{Count} veículos criados.", veiculos.Count);
            return veiculos;
        }
    }
}