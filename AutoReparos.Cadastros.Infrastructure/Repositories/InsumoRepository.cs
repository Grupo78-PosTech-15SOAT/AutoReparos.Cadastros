using AutoReparos.Cadastros.Domain.Insumos.Entities;
using AutoReparos.Cadastros.Domain.Insumos.Repositories;
using AutoReparos.Cadastros.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AutoReparos.Cadastros.Infrastructure.Repositories
{
    public class InsumoRepository : IInsumoRepository
    {
        private readonly AppDbContext _context;

        public InsumoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task Create(Insumo insumo)
        {
            await _context.Insumos.AddAsync(insumo);
            await _context.SaveChangesAsync();
        }

        public async Task<Insumo?> GetById(Guid id)
            => await _context.Insumos.FindAsync(id);

        public async Task<(IEnumerable<Insumo> Items, int Total)> GetAll(string? nome, int skip, int take)
        {
            var query = _context.Insumos.AsQueryable();

            if (!string.IsNullOrWhiteSpace(nome))
                query = query.Where(p => p.Nome.Contains(nome));

            var total = await query.CountAsync();
            var items = await query
                .OrderBy(p => p.Nome)
                .Skip(skip)
                .Take(take)
                .ToListAsync();

            return (items, total);
        }

        public async Task Update(Insumo insumo)
        {
            _context.Insumos.Update(insumo);
            await _context.SaveChangesAsync();
        }

        public async Task Delete(Insumo insumo)
        {
            _context.Insumos.Remove(insumo);
            await _context.SaveChangesAsync();
        }
    }
}
