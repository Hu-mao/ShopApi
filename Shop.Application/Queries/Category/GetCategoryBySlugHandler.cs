using AutoMapper;
using MediatR;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Queries.GetCategoryBySlug;

public class GetCategoryBySlugHandler
    : IRequestHandler<GetCategoryBySlugQuery, CategoryReadDTO?>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetCategoryBySlugHandler(
        ICategoryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryReadDTO?> Handle(
        GetCategoryBySlugQuery request,
        CancellationToken cancellationToken)
    {
        var category =
            await _repository.GetCategoryBySlugAsync(request.Slug);

        if (category == null)
            return null;

        return _mapper.Map<CategoryReadDTO>(category);
    }
}