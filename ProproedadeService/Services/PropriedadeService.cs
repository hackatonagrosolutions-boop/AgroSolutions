using PropriedadeService.Models;
using PropriedadeService.Repositories.Interface;
using PropriedadeService.Services.Interface;

namespace PropriedadeService.Services
{
    public class PropriedadeService : IPropriedadeService
    {
        private readonly IPropriedadeRepository _repo;
        public PropriedadeService(IPropriedadeRepository repo) => _repo = repo;
        public Task<IEnumerable<Propriedade>> ListarUsuariosAsync() => _repo.GetAllAsync();
        public Task<Propriedade> BuscarPorIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task CriarUsuarioAsync(Propriedade propriedade) => _repo.AddAsync(propriedade);
        public Task AtualizarUsuarioAsync(Propriedade propriedade) => _repo.UpdateAsync(propriedade);
        public Task RemoverUsuarioAsync(int id) => _repo.DeleteAsync(id);
    }
}
