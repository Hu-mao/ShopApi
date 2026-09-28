using Microsoft.Extensions.Configuration;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace RabbitMqReader;

sealed class User
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
}

internal class Program
{
    static async Task Main(string[] args)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false)
            .Build();

        var factory = new ConnectionFactory
        {
            HostName = configuration["RabbitMq:Host"] ?? "localhost",
            Port = int.Parse(configuration["RabbitMq:Port"] ?? "5672"),
            UserName = configuration["RabbitMq:UserName"] ?? "guest",
            Password = configuration["RabbitMq:Password"] ?? "guest"
        };

        var connection = await factory.CreateConnectionAsync();
        var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(
            queue: "Users",
            durable: true,
            exclusive: false,
            autoDelete: false
        );

        var consumer = new AsyncEventingBasicConsumer(channel);

        consumer.ReceivedAsync += async (sender, e) =>
        {
            var body = e.Body.ToArray();
            var json = Encoding.UTF8.GetString(body);

            var message = JsonSerializer.Deserialize<User>(json);

            if (message == null)
                return;

            Console.WriteLine("===== USER FROM QUEUE =====");
            Console.WriteLine($"Email: {message.Email}");
            Console.WriteLine($"Password: {message.Password}");
            Console.WriteLine("===========================");

            await Task.CompletedTask;
        };

        await channel.BasicConsumeAsync(
            queue: "Users",
            autoAck: true,
            consumer: consumer
        );

        Console.WriteLine("RabbitMQ Reader started.");
        Console.WriteLine("Waiting messages from Users queue...");

        Console.ReadLine();

        await channel.CloseAsync();
        await connection.CloseAsync();
    }
}