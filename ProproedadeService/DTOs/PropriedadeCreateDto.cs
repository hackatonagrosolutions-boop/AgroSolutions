using System.ComponentModel.DataAnnotations;

namespace PropriedadeService.DTOs
{
    public class PropriedadeCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Cidade é obrigatório")]
        [MaxLength(100, ErrorMessage = "Cidade deve ter no máximo 100 caracteres")]
        public string Cidade { get; set; }

        [Required(ErrorMessage = "Estado é obrigatório")]
        [MaxLength(2, ErrorMessage = "Estado deve ter no máximo 2 caracteres")]
        public string Estado { get; set; }

        [Required(ErrorMessage = "Área em hectares é obrigatório")]
        public decimal AreaTotal { get; set; }
    }
}
