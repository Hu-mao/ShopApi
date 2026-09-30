using AutoMapper;
using MediatR;
using Shop.Application.DTOs.CategoryDTOs;
using Shop.Application.Interfaces.Repository;

namespace Shop.Application.Queries.GetCategoryById;

public class GetCategoryByIdHandler
    : IRequestHandler<GetCategoryByIdQuery, CategoryReadDTO?>
{
    private readonly ICategoryRepository _repository;
    private readonly IMapper _mapper;

    public GetCategoryByIdHandler(
        ICategoryRepository repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<CategoryReadDTO?> Handle(
        GetCategoryByIdQuery request,
        CancellationToken cancellationToken)
    {
        var category =
            await _repository.GetCategoryByIdAsync(request.Id);

        if (category == null)
            return null;

        return _mapper.Map<CategoryReadDTO>(category);
    }
}