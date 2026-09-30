using MediatR;
using Shop.Application.Interfaces.Repository;
using CategoryModel = Shop.Domain.Models.Category;

namespace Shop.Application.Commands.Category;

public class CreateCategoryHandler
    : IRequestHandler<CreateCategoryCommand, int?>
{
    private readonly ICategoryRepository _repository;

    public CreateCategoryHandler(ICategoryRepository repository)
    {
        _repository = repository;
    }

    public async Task<int?> Handle(
        CreateCategoryCommand request,
        CancellationToken cancellationToken)
    {
        var category = new CategoryModel
        {
            Name = request.Dto.Name,
            Slug = request.Dto.Slug,
            Url = request.Dto.Url ?? string.Empty,
            ParentId = request.Dto.ParentId
        };

        return await _repository.AddCategoryAsync(category);
    }
}