namespace AlertaService.Models
{
    public class AlertaMensagem
    {
        public string Mensagem { get; set; }
        public int TalhaoId { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; }
        public double Vento { get; set; }
        public DateTime Data { get; set; }
    }
}
