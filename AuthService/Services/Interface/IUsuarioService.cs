using AuthService.Models;

namespace AuthService.Services.Interface
{
    public interface IUsuarioService
    {
        Task<IEnumerable<Usuario>> ListarUsuariosAsync();
        Task<Usuario> BuscarPorIdAsync(int id);
        Task CriarUsuarioAsync(Usuario usuario);
        Task AtualizarUsuarioAsync(Usuario usuario);
        Task RemoverUsuarioAsync(int id);
    }
}
