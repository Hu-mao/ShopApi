using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Enums;
using Shop.Domain.Models;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Data;
using System.Text;
using System.Text.Json;

namespace Shop.Api.Services;

public class OrderRabbitMqConsumer : BackgroundService
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly RabbitMqSettings _settings;

    private IConnection? _connection;
    private IChannel? _channel;

    public OrderRabbitMqConsumer(
        IServiceScopeFactory scopeFactory,
        IOptions<RabbitMqSettings> options)
    {
        _scopeFactory = scopeFactory;
        _settings = options.Value;
    }

    protected override async Task ExecuteAsync(
        CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.Host,
            Port = _settings.Port,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        _connection =
            await factory.CreateConnectionAsync();

        _channel =
            await _connection.CreateChannelAsync();

        await _channel.QueueDeclareAsync(
            queue: "Orders",
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null);

        var consumer =
            new AsyncEventingBasicConsumer(_channel);

        consumer.ReceivedAsync += async (sender, args) =>
        {
            try
            {
                var body = args.Body.ToArray();

                var json =
                    Encoding.UTF8.GetString(body);

                var order =
                    JsonSerializer.Deserialize<OrderMessage>(
                        json);

                if (order == null)
                {
                    await _channel.BasicNackAsync(
                        args.DeliveryTag,
                        false,
                        false);

                    return;
                }

                await ProcessOrder(order);

                await _channel.BasicAckAsync(
                    args.DeliveryTag,
                    false);
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"Помилка обробки замовлення: {ex.Message}");

                await _channel.BasicNackAsync(
                    args.DeliveryTag,
                    false,
                    false);
            }
        };

        await _channel.BasicConsumeAsync(
            queue: "Orders",
            autoAck: false,
            consumer: consumer);

        Console.WriteLine(
            "OrderRabbitMqConsumer запущений.");

        await Task.Delay(
            Timeout.Infinite,
            stoppingToken);
    }

    private async Task ProcessOrder(
        OrderMessage message)
    {
        using var scope =
            _scopeFactory.CreateScope();

        var db =
            scope.ServiceProvider
                .GetRequiredService<ShopDbContext>();

        var emailService =
            scope.ServiceProvider
                .GetRequiredService<IEmailService>();

        var user =
            await db.Users
                .FirstOrDefaultAsync(
                    x => x.Id == message.UserId);

        if (user == null)
        {
            await emailService.SendEmailAsync(
                message.Email,
                "Помилка замовлення",
                "Користувача не знайдено.");

            return;
        }

        var productIds =
            message.Products
                .Select(x => x.ProductId)
                .Distinct()
                .ToList();

        var products =
            await db.Products
                .Where(x =>
                    productIds.Contains(x.Id))
                .ToListAsync();

        var missingProducts =
            message.Products
                .Where(x =>
                {
                    var product =
                        products.FirstOrDefault(
                            p => p.Id == x.ProductId);

                    return product == null ||
                           !product.IsActive ||
                           product.StockQty < x.Quantity;
                })
                .ToList();

        if (missingProducts.Count > 0)
        {
            await emailService.SendEmailAsync(
                message.Email,
                "Замовлення очікує",
                """
                <h2>Замовлення очікує</h2>
                <p>На складі недостатньо товару.</p>
                <p>Ми повідомимо вас після появи товару.</p>
                """);

            return;
        }

        await using var transaction =
            await db.Database.BeginTransactionAsync();

        try
        {
            var order = new Order
            {
                UserId = message.UserId,
                Status = OrderStatus.Confirmed,
                Paid = false
            };

            db.Orders.Add(order);

            await db.SaveChangesAsync();

            decimal totalPrice = 0;

            var emailBody = new StringBuilder();

            emailBody.Append(
                "<h2>Ваше замовлення підтверджено</h2>");

            emailBody.Append(
                "<table border='1' cellpadding='5'>");

            emailBody.Append(
                "<tr>" +
                "<th>Товар</th>" +
                "<th>Ціна</th>" +
                "<th>Кількість</th>" +
                "<th>Сума</th>" +
                "</tr>");

            foreach (var item in message.Products)
            {
                var product =
                    products.First(
                        x => x.Id == item.ProductId);

                var sum =
                    product.Price * item.Quantity;

                totalPrice += sum;

                product.StockQty -= item.Quantity;

                var detail = new OrderDetail
                {
                    OrderId = order.Id,

                    ProductId = product.Id,

                    Price = product.Price,

                    Count = item.Quantity
                };

                db.OrderDetails.Add(detail);

                emailBody.Append(
                    $"<tr>" +
                    $"<td>{product.Name}</td>" +
                    $"<td>{product.Price}</td>" +
                    $"<td>{item.Quantity}</td>" +
                    $"<td>{sum}</td>" +
                    $"</tr>");
            }

            await db.SaveChangesAsync();

            await transaction.CommitAsync();

            emailBody.Append("</table>");

            emailBody.Append(
                $"<h3>Загальна ціна: {totalPrice}</h3>");

            await emailService.SendEmailAsync(
                message.Email,
                "Замовлення підтверджено",
                emailBody.ToString());
        }
        catch
        {
            await transaction.RollbackAsync();

            throw;
        }
    }

    public override async Task StopAsync(
        CancellationToken cancellationToken)
    {
        if (_channel != null)
            await _channel.CloseAsync();

        if (_connection != null)
            await _connection.CloseAsync();

        await base.StopAsync(cancellationToken);
    }
}