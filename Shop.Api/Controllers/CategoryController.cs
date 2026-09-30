using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Shop.Api.Interfaces;
using Shop.Api.Request.Category;
using Shop.Application.Commands.Category;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Application.Queries.GetCategoryById;
using Shop.Application.Queries.GetCategoryBySlug;

namespace Shop.Api.Controllers;

[ApiController]
[Route("api/v1/[controller]")] //https://ip:port/api/v1
public class CategoryController(
    ICategoryService _categoryService,
    IImageService _imageService,
    IMediator _mediator) : ControllerBase
{
    [Authorize]
    [HttpPost]
    public async Task<IActionResult> CreateCategory([FromForm] CategoryCreateRequest dto)
    {
        string? url = null;

        if (dto.Image != null)
        {
            url = await _imageService.SaveFileAsync(dto.Image, "categories");
        }

        var createDto = new CategoryCreateDTO
        {
            Name = dto.Name,
            Url = url,
            Slug = dto.Slug,
            ParentId = dto.ParentId,
        };

        var id = await _mediator.Send(
    new CreateCategoryCommand(createDto));

        return Ok($"Category created {id}");
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCategories(
    [FromQuery] int page = 1,
    [FromQuery] int pageSize = 8)
    {
        var result = await _categoryService
            .GetCategoriesPagedAsync(page, pageSize);

        return Ok(result);
    }
    [HttpPut]
    public async Task<IActionResult> UpdateCategory([FromBody] CategoryUpdateDTO dto)
    {
        var result = await _categoryService.UpdateCategoryAsync(dto);

        if (!result)
            return NotFound();

        return NoContent();
    }
    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoryService.DeleteCategoryAsync(id);

        if (!result)
            return NotFound();

        return NoContent();
    }
    [HttpGet("{id}/parents")]
    public async Task<IActionResult> GetParentCategories(int id)
    {
        var categories = await _categoryService.GetParentCategoriesAsync(id);

        if (categories == null)
            return NotFound();

        return Ok(categories);
    }
    [HttpGet("{id}/children")]
    public async Task<IActionResult> GetChildCategories(int id)
    {
        var categories = await _categoryService.GetChildCategoriesAsync(id);

        if (categories == null)
            return NotFound();

        return Ok(categories);
    }
    [HttpGet("tree")]
    public async Task<IActionResult> GetCategoryTree()
    {
        var tree = await _categoryService.GetCategoryTreeAsync();

        if (tree.Count == 0)
            return NotFound();

        return Ok(tree);
    }
    [HttpGet("slug/{slug}")]
    public async Task<ActionResult<CategoryReadDTO>> GetCategoryBySlug(
    string slug)
    {
        var dto = await _mediator.Send(
            new GetCategoryBySlugQuery(slug));

        if (dto == null)
            return NotFound();

        return Ok(dto);
    }
}
