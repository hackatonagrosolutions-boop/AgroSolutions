using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PropriedadeService.Models
{
    public class Talhao
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cultura { get; set; }

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } //(ex: "Normal", "Risco de Praga")

        [Required]
        public Decimal AreaHectares { get; set; }

        [Required]
        public int PropriedadeId { get; set; }

        [ForeignKey("PropriedadeId")]
        public Propriedade Propriedade { get; set; }
    }
}
