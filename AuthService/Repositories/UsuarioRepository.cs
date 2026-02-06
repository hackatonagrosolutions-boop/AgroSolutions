using AuthService.Data;
using AuthService.Models;
using AuthService.Repositories.Interface;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Repositories
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly UsuariosDbContext _context;
        public UsuarioRepository(UsuariosDbContext context) => _context = context;
        public async Task<IEnumerable<Usuario>> GetAllAsync() => await _context.Usuarios.ToListAsync();
        public async Task<Usuario> GetByIdAsync(int id) => await _context.Usuarios.FindAsync(id);
        public async Task AddAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
        public async Task AddRangeAsync(IEnumerable<Usuario> usuarios)
        {
            await _context.Usuarios.AddRangeAsync(usuarios);
            await _context.SaveChangesAsync();
        }

        public async Task<Usuario> GetByEmailAsync(string email)
        {
            return await _context.Usuarios.FirstOrDefaultAsync(u => u.Email == email);
        }
    }
}
