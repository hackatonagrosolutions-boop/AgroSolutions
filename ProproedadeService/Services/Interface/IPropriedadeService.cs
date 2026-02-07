using PropriedadeService.Models;

namespace PropriedadeService.Services.Interface
{
    public interface IPropriedadeService
    {
        Task<IEnumerable<Propriedade>> ListarUsuariosAsync();
        Task<Propriedade> BuscarPorIdAsync(int id);
        Task CriarUsuarioAsync(Propriedade propriedade);
        Task AtualizarUsuarioAsync(Propriedade propriedade);
        Task RemoverUsuarioAsync(int id);
    }
}
