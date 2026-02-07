using PropriedadeService.Models;

namespace PropriedadeService.PropriedadeController.Interface
{
    public interface IPropriedadeController
    {
        Task<IEnumerable<Propriedade>> ListarPropriedadeAsync();
        Task<Propriedade> BuscarPorIdAsync(int id);
        Task CriarPropriedadeAsync(Propriedade propriedade);
        Task AtualizarPropriedadeAsync(Propriedade propriedade);
        Task RemoverPropriedadeAsync(int id);
    }
}
