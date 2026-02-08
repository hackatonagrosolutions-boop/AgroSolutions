using PropriedadeService.Controller.Interface;
using PropriedadeService.Models;
using PropriedadeService.Repositories.Interface;

namespace PropriedadeService.Controller
{
    public class TalhaoController : ITalhaoController
    {
        private readonly ITalhaoRepository _repo;
        public TalhaoController(ITalhaoRepository repo) => _repo = repo;
        public Task<IEnumerable<Talhao>> ListarTalhaoAsync() => _repo.GetAllAsync();
        public Task<Talhao> BuscarPorIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task CriarTalhaoAsync(Talhao talhao) => _repo.AddAsync(talhao);
        public Task AtualizarTalhaoAsync(Talhao talhao) => _repo.UpdateAsync(talhao);
        public Task RemoverTalhaoAsync(int id) => _repo.DeleteAsync(id);
    }
}
