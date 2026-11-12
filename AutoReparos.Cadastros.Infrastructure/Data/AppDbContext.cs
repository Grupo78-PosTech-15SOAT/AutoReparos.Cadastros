using AutoReparos.Cadastros.Domain.Clientes.Entities;
using AutoReparos.Cadastros.Domain.Notificacoes.Entities;
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
        public DbSet<UsuarioIdentity> Usuarios { get; set; }
        public DbSet<NotificacaoEnviada> NotificacoesEnviadas { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        }
    }
}