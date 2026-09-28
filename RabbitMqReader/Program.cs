using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMqReader;

class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var host = configuration["RabbitMq:Host"] ?? "localhost";
        var port = int.Parse(configuration["RabbitMq:Port"] ?? "5672");
        var username = configuration["RabbitMq:UserName"] ?? "guest";
        var password = configuration["RabbitMq:Password"] ?? "guest";

        var factory = new ConnectionFactory
        {
            HostName = host,
            Port = port,
            UserName = username,
            Password = password
        };

        await using var connection = await factory.CreateConnectionAsync();
        await using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "Users",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);

            Console.WriteLine("Отримано користувача:");
            Console.WriteLine(message);
            Console.WriteLine();

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "Users",
            autoAck: true,
            consumer: consumer);

        Console.WriteLine("RabbitMqReader запущений...");
        Console.ReadLine();
    }
}