using AuthService.Models;
using AuthService.Repositories.Interface;
using AuthService.Services.Interface;

namespace AuthService.Services
{
    public class UsuarioService : IUsuarioService
    {
        private readonly IUsuarioRepository _repo;
        public UsuarioService(IUsuarioRepository repo) => _repo = repo;
        public Task<IEnumerable<Usuario>> ListarUsuariosAsync() => _repo.GetAllAsync();
        public Task<Usuario> BuscarPorIdAsync(int id) => _repo.GetByIdAsync(id);
        public Task CriarUsuarioAsync(Usuario usuario) => _repo.AddAsync(usuario);
        public Task AtualizarUsuarioAsync(Usuario usuario) => _repo.UpdateAsync(usuario);
        public Task RemoverUsuarioAsync(int id) => _repo.DeleteAsync(id);
    }
}
