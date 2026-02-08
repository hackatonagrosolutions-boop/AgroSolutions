using AlertaService.Data;
using AlertaService.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

public class AlertaWorker : BackgroundService
{
    private readonly ILogger<AlertaWorker> _logger;
    private readonly IServiceScopeFactory _scopeFactory;

    public AlertaWorker(ILogger<AlertaWorker> logger, IServiceScopeFactory scopeFactory)
    {
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory { HostName = "localhost" };
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: "fila_alertas", durable: true, exclusive: false, autoDelete: false);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var messageJson = Encoding.UTF8.GetString(body);
            var dados = JsonSerializer.Deserialize<AlertaMensagem>(messageJson);

            if (dados != null)
            {
                using (var scope = _scopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<AlertaDbContext>();

                    var novoAlerta = new Alerta
                    {
                        TalhaoId = dados.TalhaoId,
                        Mensagem = dados.Mensagem,
                        UmidadeSolo = dados.UmidadeSolo,
                        Temperatura = dados.Temperatura,
                        Vento = dados.Vento,
                        DataAlerta = dados.Data
                    };

                    dbContext.Alertas.Add(novoAlerta);
                    await dbContext.SaveChangesAsync();

                    _logger.LogInformation($"Alerta do Talhão {dados.TalhaoId} gravado no SQL Server!");
                }
            }
        };

        await channel.BasicConsumeAsync(queue: "fila_alertas", autoAck: true, consumer: consumer);
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }
}