using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AuthService.DTOs;
using AuthService.Models;
using AuthService.Repositories.Interface;
using AuthService.Services.Interface;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace AuthService.Services
{
    public class AuthenticationService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly JwtSettings _jwtSettings;

        public AuthenticationService(
            IUsuarioRepository usuarioRepository,
            IOptions<JwtSettings> jwtSettings)
        {
            _usuarioRepository = usuarioRepository;
            _jwtSettings = jwtSettings.Value;
        }

        public async Task<LoginResponseDto> LoginAsync(LoginDto loginDto)
        {
            // Buscar usuário por email
            var usuario = await _usuarioRepository.GetByEmailAsync(loginDto.Email);

            if (usuario == null)
                throw new UnauthorizedAccessException("Email ou senha inválidos");

            // Verificar senha
            bool senhaValida = BCrypt.Net.BCrypt.Verify(loginDto.Senha, usuario.SenhaHash);

            if (!senhaValida)
                throw new UnauthorizedAccessException("Email ou senha inválidos");

            // Gerar token
            var token = GerarToken(usuario);
            var expiresAt = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes);

            return new LoginResponseDto
            {
                Token = token,
                Email = usuario.Email,
                Nome = usuario.Nome,
                Perfil = usuario.Perfil,
                ExpiresAt = expiresAt
            };
        }

        public string GerarToken(Usuario usuario)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_jwtSettings.SecretKey);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, usuario.Id.ToString()),
                new Claim(ClaimTypes.Email, usuario.Email),
                new Claim(ClaimTypes.Name, usuario.Nome),
                new Claim(ClaimTypes.Role, usuario.Perfil),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(_jwtSettings.ExpirationInMinutes),
                Issuer = _jwtSettings.Issuer,
                Audience = _jwtSettings.Audience,
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}