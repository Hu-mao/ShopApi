using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Filters;
using Shop.Application.Commands.Product;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.DTOs.ProductFeedbackDTOs;
using Shop.Application.Interfaces.Services;
namespace Shop.Api.Controllers;
//URL - Uniform Resource Locator - текстовий рядок, який вказує
//на місце розташування ресурса

[ApiController]
[Route("api/[controller]")]
[LogActionFilter]
public class ProductController(
    IProductService _productService,
    IProductFeedbackService _feedbackService,
    IMediator _mediator) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> Create(
    [FromBody] ProductCreateDTO dto)
    {
        var id = await _mediator.Send(
            new CreateProductCommand(dto));

        return CreatedAtAction(
            nameof(GetById),
            new { id },
            null);
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var products = await _productService.GetAllAsync();

        return Ok(products);
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetById(int id)
    {
        var product = await _productService.GetByIdAsync(id);

        if (product == null)
            return NotFound();

        return Ok(product);
    }

    [HttpPost("{productId:int}/feedback")]
    public async Task<IActionResult> AddFeedback(
    int productId,
    [FromBody] ProductFeedbackCreateDTO dto)
    {
        var product = await _productService.GetByIdAsync(productId);

        if (product == null)
            return NotFound("Product not found");

        if (dto.Type != "Review" && dto.Type != "Question")
            return BadRequest("Type must be Review or Question");

        if (string.IsNullOrWhiteSpace(dto.Text))
            return BadRequest("Text is required");

        if (dto.Type == "Review" &&
            (dto.Rating == null || dto.Rating < 1 || dto.Rating > 5))
            return BadRequest("Rating must be from 1 to 5");

        if (dto.Type == "Question")
            dto.Rating = null;

        await _feedbackService.AddAsync(productId, dto);

        return Ok(new
        {
            message = "Feedback added successfully"
        });
    }
}
