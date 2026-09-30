using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.GetCategoryById;

public record GetCategoryByIdQuery(int Id)
    : IRequest<CategoryReadDTO?>;