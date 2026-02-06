using AuthService.DTOs;
using AuthService.Models;

namespace AuthService.Services.Interface
{
    public interface IAuthService
    {
        Task<LoginResponseDto> LoginAsync(LoginDto loginDto);
        string GerarToken(Usuario usuario);
    }
}
