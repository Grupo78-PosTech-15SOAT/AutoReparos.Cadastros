using AutoReparos.Cadastros.Domain.Notificacoes.Entities;
using AutoReparos.Cadastros.Domain.Notificacoes.Repositories;
using AutoReparos.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace AutoReparos.Cadastros.Infrastructure.Repositories
{
    public class NotificacaoEnviadaRepository(AppDbContext context) : INotificacaoEnviadaRepository
    {
        public Task<bool> Existe(Guid eventId) =>
            context.NotificacoesEnviadas.AnyAsync(n => n.EventId == eventId);

        public async Task<bool> Registrar(NotificacaoEnviada notificacao)
        {
            try
            {
                context.NotificacoesEnviadas.Add(notificacao);
                await context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateException ex) when (ex.InnerException is PostgresException { SqlState: PostgresErrorCodes.UniqueViolation })
            {
                context.Entry(notificacao).State = EntityState.Detached;
                return false;
            }
        }
    }
}
