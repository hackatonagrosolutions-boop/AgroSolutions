using System.ComponentModel.DataAnnotations;

namespace AuthService.DTOs
{
    public class UsuarioCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(200, ErrorMessage = "Nome deve ter no máximo 200 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Email é obrigatório")]
        [EmailAddress(ErrorMessage = "Email em formato inválido")]
        [MaxLength(100, ErrorMessage = "Email deve ter no máximo 100 caracteres")]
        public string Email { get; set; }

        [Required(ErrorMessage = "Senha é obrigatória")]
        [MinLength(6, ErrorMessage = "Senha deve ter no mínimo 6 caracteres")]
        [MaxLength(100, ErrorMessage = "Senha deve ter no máximo 100 caracteres")]
        public string Senha { get; set; }

        [Required(ErrorMessage = "Perfil é obrigatório")]
        [MaxLength(250, ErrorMessage = "Perfil deve ter no máximo 50 caracteres")]
        public string Perfil { get; set; }
    }
}
