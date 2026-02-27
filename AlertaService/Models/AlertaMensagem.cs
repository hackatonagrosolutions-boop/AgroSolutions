namespace AlertaService.Models
{
    public class AlertaMensagem
    {
        public string Mensagem { get; set; }
        public int TalhaoId { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; }
        public double Vento { get; set; }
        public double Chuva { get; set; }
        public string TipoAlerta { get; set; } // Chuva, Vento, Temperatura, Solo
        public DateTime Data { get; set; }
    }
}
