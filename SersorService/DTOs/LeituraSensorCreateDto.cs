namespace SersorService.DTOs
{
    public class LeituraSensorCreateDto
    {
        public int TalhaoId { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; } // em Celsius
        public double Vento { get; set; } // em km/h
        public double Chuva { get; set; } // em mm
        public DateTime? DataLeitura { get; set; }
    }
}
