using System.ComponentModel.DataAnnotations;

namespace PropriedadeService.DTOs
{
    public class TalhaoUpdateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Cultura é obrigatório")]
        [MaxLength(100, ErrorMessage = "Cultura deve ter no máximo 100 caracteres")]
        public string Cultura { get; set; }

        [Required(ErrorMessage = "Área em hectares é obrigatório")]
        public Decimal AreaHectares { get; set; }

        [MaxLength(100, ErrorMessage = "Status deve ter no máximo 50 caracteres")]
        public string Status { get; set; }
    }
}
