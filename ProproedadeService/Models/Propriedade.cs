using System.ComponentModel.DataAnnotations;

namespace PropriedadeService.Models
{
    public class Propriedade
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }

        [Required]
        [MaxLength(100)]
        public string Cidade { get; set; }

        [Required]
        [MaxLength(2)]
        public string Estado { get; set; }

        [Required]
        public Decimal AreaHectares { get; set; }

        public ICollection<Talhao> Talhoes { get; set; } = new List<Talhao>();
    }
}
