using Microsoft.AspNetCore.Mvc;
using Shop.Domain.Models;

[ApiController]
[Route("api/v1/[controller]")]
public class DeliveryAddressController(
    IDeliveryAddressService _service) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAll(
        CancellationToken cancellationToken)
    {
        // отримуємо поточного користувача

        var result = await _service.GetAllAsync(
            userId,
            cancellationToken);

        return Ok(result);
    }
}