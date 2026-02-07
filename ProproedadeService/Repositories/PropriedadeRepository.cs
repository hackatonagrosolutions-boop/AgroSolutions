using Microsoft.EntityFrameworkCore;
using PropriedadeService.Data;
using PropriedadeService.Models;
using PropriedadeService.Repositories.Interface;

namespace PropriedadeService.Repositories
{
    public class PropriedadeRepository : IPropriedadeRepository
    {
        private readonly PropriedadesDbContext _context;
        public PropriedadeRepository(PropriedadesDbContext context) => _context = context;
        public async Task<IEnumerable<Propriedade>> GetAllAsync() => await _context.Propriedades.ToListAsync();
        public async Task<Propriedade> GetByIdAsync(int id) => await _context.Propriedades.FindAsync(id);
        public async Task AddAsync(Propriedade propriedade)
        {
            _context.Propriedades.Add(propriedade);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Propriedade propriedade)
        {
            _context.Propriedades.Update(propriedade);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var propriedade = await _context.Propriedades.FindAsync(id);
            if (propriedade != null)
            {
                _context.Propriedades.Remove(propriedade);
                await _context.SaveChangesAsync();
            }
        }
    }
}
