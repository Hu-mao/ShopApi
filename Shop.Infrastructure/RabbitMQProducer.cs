using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Infrastructure.Configuration;
using System.Text;
using System.Text.Json;

namespace Shop.Infrastructure.RabbitMQ;

public class RabbitMQProducer
{
    private readonly RabbitMqSettings _settings;

    public RabbitMQProducer(
        IOptions<RabbitMqSettings> options)
    {
        _settings = options.Value;
    }

    public async Task SendOrderAsync(
        OrderMessage order)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        await using var connection =
            await factory.CreateConnectionAsync();

        await using var channel =
            await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "Orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var json = JsonSerializer.Serialize(order);

        var body = Encoding.UTF8.GetBytes(json);

        var properties = new BasicProperties
        {
            Persistent = true
        };

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: "Orders",
            mandatory: false,
            basicProperties: properties,
            body: body);
    }
}