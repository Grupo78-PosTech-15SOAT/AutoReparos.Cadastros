using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Insumos.Entities;
using AutoReparos.Cadastros.Domain.OrdensServicos.Entities;
using AutoReparos.Cadastros.Domain.Servicos.Entities;
using AutoReparos.Cadastros.Domain.Veiculos.Entities;
using AutoReparos.Cadastros.Infrastructure.Identity.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection;

namespace AutoReparos.Cadastros.Infrastructure.Data
{
    public class AppDbContext(DbContextOptions options) : IdentityDbContext<UsuarioIdentity, IdentityRole<Guid>, Guid>(options)
    {
        public DbSet<Cliente> Clientes { get; set; }
        public DbSet<Veiculo> Veiculos { get; set; }
        public DbSet<Servico> Servicos { get; set; }
        public DbSet<Insumo> Insumos { get; set; }
        public DbSet<OrdemServico> OrdensServico { get; set; }
        public DbSet<OrdemServicoServico> OrdensServicoServicos { get; set; }
        public DbSet<OrdemServicoInsumo> OrdensServicoInsumos { get; set; }
        public DbSet<UsuarioIdentity> Usuarios { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}