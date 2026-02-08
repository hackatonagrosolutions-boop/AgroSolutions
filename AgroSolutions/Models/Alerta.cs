namespace AlertaService.Models
{
    public class Alerta
    {
        public int Id { get; set; }
        public int TalhaoId { get; set; }
        public string Mensagem { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; }
        public double Vento { get; set; }
        public DateTime DataAlerta { get; set; }
    }
}
