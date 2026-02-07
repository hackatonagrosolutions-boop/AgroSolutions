using System.ComponentModel.DataAnnotations;

namespace PropriedadeService.DTOs
{
    public class TalhaoCreateDto
    {
        [Required(ErrorMessage = "Nome é obrigatório")]
        [MaxLength(100, ErrorMessage = "Nome deve ter no máximo 100 caracteres")]
        public string Nome { get; set; }

        [Required(ErrorMessage = "Cultura é obrigatório")]
        [MaxLength(100, ErrorMessage = "Cultura deve ter no máximo 100 caracteres")]
        public string Cultura { get; set; }

        [Required(ErrorMessage = "Área em hectares é obrigatório")]
        public Decimal AreaHectares { get; set; }

        // O campo de status será atualizdo posteriormente com base nos dados de monitoramento

        // Relacionamento com Propriedade
        public int PropriedadeId { get; set; }

    }
}
