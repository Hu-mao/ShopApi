using MediatR;
using Shop.Application.DTOs.CategoryDTOs;

namespace Shop.Application.Queries.GetCategoryBySlug;

public record GetCategoryBySlugQuery(string Slug)
    : IRequest<CategoryReadDTO?>;