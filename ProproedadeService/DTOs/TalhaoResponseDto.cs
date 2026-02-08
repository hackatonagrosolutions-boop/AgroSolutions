namespace PropriedadeService.DTOs
{
    public class TalhaoResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cultura { get; set; }
        public string Status { get; set; }
        public decimal AreaHectares { get; set; }
        public int PropriedadeId { get; set; }
    }
}
