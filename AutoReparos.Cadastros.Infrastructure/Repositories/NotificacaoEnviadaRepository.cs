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

        public async Task<T> ComTrava<T>(Guid eventId, Func<Task<T>> acao)
        {
            // Trava de sessão numa conexão própria: o EF usa outra conexão dentro da ação, sem se bloquear
            await using var conexao = new NpgsqlConnection(context.Database.GetConnectionString());
            await conexao.OpenAsync();

            await ExecutarAsync(conexao, "SELECT pg_advisory_lock(hashtextextended(@chave, 0))", eventId);
            try
            {
                return await acao();
            }
            finally
            {
                await ExecutarAsync(conexao, "SELECT pg_advisory_unlock(hashtextextended(@chave, 0))", eventId);
            }
        }

        private static async Task ExecutarAsync(NpgsqlConnection conexao, string sql, Guid eventId)
        {
            await using var comando = new NpgsqlCommand(sql, conexao);
            comando.Parameters.AddWithValue("chave", eventId.ToString());
            await comando.ExecuteScalarAsync();
        }

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
