using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Commands.Category;

public record CreateCategoryCommand(
    CategoryCreateDTO Dto
) : IRequest<int?>;