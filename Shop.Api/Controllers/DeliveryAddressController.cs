using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Application.DTOs.DeliveryAddress;
using Shop.Application.Services;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize]
public class DeliveryAddressController : ControllerBase
{
    private readonly DeliveryAddressService _service;

    public DeliveryAddressController(DeliveryAddressService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<DeliveryAddressReadDto>>> GetAll(
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var addresses = await _service.GetAllAsync(
            userId,
            cancellationToken);

        return Ok(addresses);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<DeliveryAddressReadDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var address = await _service.GetByIdAsync(
            id,
            userId,
            cancellationToken);

        if (address == null)
            return NotFound();

        return Ok(address);
    }

    [HttpPost]
    public async Task<ActionResult<DeliveryAddressReadDto>> Create(
        [FromBody] DeliveryAddressCreateDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var address = await _service.CreateAsync(
            userId,
            dto,
            cancellationToken);

        return CreatedAtAction(
            nameof(GetById),
            new { id = address.Id },
            address);
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] DeliveryAddressUpdateDto dto,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _service.UpdateAsync(
            id,
            userId,
            dto,
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken)
    {
        var userId = GetUserId();

        var result = await _service.DeleteAsync(
            id,
            userId,
            cancellationToken);

        if (!result)
            return NotFound();

        return NoContent();
    }

    private Guid GetUserId()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userId))
            throw new UnauthorizedAccessException("User ID not found.");

        return Guid.Parse(userId);
    }
}