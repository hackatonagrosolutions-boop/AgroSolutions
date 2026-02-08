using RabbitMQ.Client;
using System.Text;
using System.Text.Json;

public class MensageriaService
{
    private readonly string _hostname = "localhost";

    public async Task PublicarAlertaAsync(object alerta)
    {
        var factory = new ConnectionFactory { HostName = _hostname };

        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "fila_alertas",
                                        durable: true,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);

        var message = JsonSerializer.Serialize(alerta);
        var body = Encoding.UTF8.GetBytes(message);
        await channel.BasicPublishAsync(exchange: string.Empty,routingKey: "fila_alertas", body: body);
    }
}