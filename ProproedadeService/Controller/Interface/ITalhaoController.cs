using PropriedadeService.Models;

namespace PropriedadeService.Controller.Interface
{
    public interface ITalhaoController
    {
        Task<IEnumerable<Talhao>> ListarTalhaoAsync();
        Task<Talhao> BuscarPorIdAsync(int id);
        Task CriarTalhaoAsync(Talhao talhao);
        Task AtualizarTalhaoAsync(Talhao talhao);
        Task RemoverTalhaoAsync(int id);
    }
}
