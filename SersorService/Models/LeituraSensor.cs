using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace SersorService.Models
{
    public class LeituraSensor
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }
        public int TalhaoId { get; set; }
        public double UmidadeSolo { get; set; }
        public double Temperatura { get; set; }
        public double Vento { get; set; }
        public double Chuva { get; set; }
        public string TipoAlerta { get; set; } // Chuva, Vento, Temperatura, Solo
        public DateTime DataLeitura { get; set; } = DateTime.UtcNow;
    }
}
