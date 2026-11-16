using AutoReparos.Cadastros.Domain.Notificacoes.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AutoReparos.Cadastros.Infrastructure.Data.Mappings
{
    public class NotificacaoEnviadaMapping : IEntityTypeConfiguration<NotificacaoEnviada>
    {
        public void Configure(EntityTypeBuilder<NotificacaoEnviada> builder)
        {
            builder.ToTable("NotificacoesEnviadas");
            builder.HasKey(n => n.EventId);
            builder.Property(n => n.EventId).ValueGeneratedNever();
            builder.Property(n => n.Tipo).IsRequired().HasMaxLength(100);
            builder.Property(n => n.ClienteId).IsRequired();
            builder.Property(n => n.EnviadoEm).IsRequired();
        }
    }
}
