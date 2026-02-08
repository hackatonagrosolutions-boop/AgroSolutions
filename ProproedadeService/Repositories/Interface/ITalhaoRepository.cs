using PropriedadeService.Models;

namespace PropriedadeService.Repositories.Interface
{
    public interface ITalhaoRepository
    {
        Task<IEnumerable<Talhao>> GetAllAsync();
        Task<Talhao> GetByIdAsync(int id);
        Task AddAsync(Talhao talhao);
        Task UpdateAsync(Talhao talhao);
        Task DeleteAsync(int id);
    }
}
