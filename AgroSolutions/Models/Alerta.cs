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
        public double Chuva { get; set; }
        public string TipoAlerta { get; set; } // Chuva, Vento, Temperatura, Solo
        public DateTime DataAlerta { get; set; }
    }
}
