namespace PropriedadeService.DTOs
{
    public class PropriedadeResponseDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public decimal AreaHectares { get; set; }
        public List<TalhaoResponseDto> Talhoes { get; set; }
    }
}
