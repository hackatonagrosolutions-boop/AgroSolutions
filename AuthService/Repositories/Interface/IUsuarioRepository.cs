using AuthService.Models;

namespace AuthService.Repositories.Interface
{
    public interface IUsuarioRepository
    {
        Task<IEnumerable<Usuario>> GetAllAsync();
        Task<Usuario> GetByIdAsync(int id);
        Task<Usuario> GetByEmailAsync(string email);
        Task AddAsync(Usuario usuario);
        Task AddRangeAsync(IEnumerable<Usuario> usuarios);
        Task UpdateAsync(Usuario usuario);
        Task DeleteAsync(int id);
    }
}
