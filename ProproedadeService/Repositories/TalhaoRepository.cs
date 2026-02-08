using Microsoft.EntityFrameworkCore;
using PropriedadeService.Data;
using PropriedadeService.Models;
using PropriedadeService.Repositories.Interface;

namespace PropriedadeService.Repositories
{
    public class TalhaoRepository : ITalhaoRepository
    {
        private readonly PropriedadesDbContext _context;
        public TalhaoRepository(PropriedadesDbContext context) => _context = context;
        
        //public async Task<IEnumerable<Talhao>> GetAllAsync() => await _context.Talhoes.ToListAsync();
        public async Task<IEnumerable<Talhao>> GetAllAsync()
        {
            return await _context.Talhoes
                .Include(t => t.Propriedade)
                .ToListAsync();
        }
        
        //public async Task<Talhao> GetByIdAsync(int id) => await _context.Talhoes.FindAsync(id);
        public async Task<Talhao> GetByIdAsync(int id)
        {
            return await _context.Talhoes
                .Include(t => t.Propriedade)
                .FirstOrDefaultAsync(t => t.Id == id);
        }
     
        public async Task AddAsync(Talhao talhao)
        {
            _context.Talhoes.Add(talhao);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Talhao talhao)
        {
            _context.Talhoes.Update(talhao);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var talhao = await _context.Talhoes.FindAsync(id);
            if (talhao != null)
            {
                _context.Talhoes.Remove(talhao);
                await _context.SaveChangesAsync();
            }
        }
    }
}
