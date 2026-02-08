namespace PropriedadeService.DTOs
{
    public class PropriedadeResponseResumoDto
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Cidade { get; set; }
        public string Estado { get; set; }
        public decimal AreaHectares { get; set; }
        public List<TalhaoResponseResumoDto> Talhoes { get; set; } = new();
    }
}
