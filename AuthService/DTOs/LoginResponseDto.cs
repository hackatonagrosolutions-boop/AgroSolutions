namespace AuthService.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public string Email { get; set; }
        public string Nome { get; set; }
        public string Perfil { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
