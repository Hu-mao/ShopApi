using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.OrderDTOs;
using Shop.Infrastructure.RabbitMQ;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly RabbitMQProducer _rabbitMQProducer;

    public OrderController(RabbitMQProducer rabbitMQProducer)
    {
        _rabbitMQProducer = rabbitMQProducer;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        [FromBody] OrderCreateDTO dto)
    {
        if (dto.UserId == Guid.Empty)
            return BadRequest("Некоректний UserId.");

        if (dto.Products == null || dto.Products.Count == 0)
            return BadRequest("Замовлення повинно містити хоча б один продукт.");

        if (dto.Products.Any(x =>
            x.ProductId <= 0 ||
            x.Quantity <= 0))
        {
            return BadRequest(
                "ProductId та Quantity повинні бути більшими за 0.");
        }

        if (string.IsNullOrWhiteSpace(dto.Email))
            return BadRequest("Email є обов'язковим.");

        var message = new OrderMessage
        {
            UserId = dto.UserId,

            FirstName = dto.FirstName,

            LastName = dto.LastName,

            Email = dto.Email,

            Phone = dto.Phone,

            Address = dto.Address,

            City = dto.City,

            Products = dto.Products
        };

        await _rabbitMQProducer.SendOrderAsync(message);

        return Accepted(new
        {
            message = "Замовлення додано в чергу Orders."
        });
    }
}