using PropriedadeService.Models;
using PropriedadeService.PropriedadeController.Interface;
using PropriedadeService.Repositories.Interface;


namespace PropriedadeService.PropriedadeController
{
    public class PropriedadeController : IPropriedadeController
    {
        private readonly IPropriedadeRepository _repo;
        public PropriedadeController(IPropriedadeRepository repo) => _repo = repo;
        public Task<IEnumerable<Propriedade>> ListarPropriedadeAsync() => _repo.GetAllAsync();
        public Task<Propriedade> BuscarPorIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task CriarPropriedadeAsync(Propriedade propriedade) => _repo.AddAsync(propriedade);
        public Task AtualizarPropriedadeAsync(Propriedade propriedade) => _repo.UpdateAsync(propriedade);
        public Task RemoverPropriedadeAsync(int id) => _repo.DeleteAsync(id);
    }
}
