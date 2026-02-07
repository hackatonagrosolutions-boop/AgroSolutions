using PropriedadeService.Models;

namespace PropriedadeService.Repositories.Interface
{
    public interface IPropriedadeRepository
    {
        Task<IEnumerable<Propriedade>> GetAllAsync();
        Task<Propriedade> GetByIdAsync(int id);
        Task AddAsync(Propriedade propriedade);
        Task UpdateAsync(Propriedade propriedade);
        Task DeleteAsync(int id);
    }
}
