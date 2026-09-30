using MediatR;
using Shop.Application.Interfaces.Repository;
using ProductModel = Shop.Domain.Models.Product;
using ProductImageModel = Shop.Domain.Models.ProductImage;

namespace Shop.Application.Commands.Product;

public class CreateProductHandler
    : IRequestHandler<CreateProductCommand, int>
{
    private readonly IProductRepository _repository;

    public CreateProductHandler(IProductRepository repository)
    {
        _repository = repository;
    }

    public async Task<int> Handle(
        CreateProductCommand request,
        CancellationToken cancellationToken)
    {
        var dto = request.Dto;

        var product = new ProductModel
        {
            Name = dto.Name,
            Description = dto.Description,
            Price = dto.Price,
            StockQty = dto.StockQty,
            CategoryId = dto.CategoryId,
            Images = dto.Images
                .Select((url, index) => new ProductImageModel
                {
                    Url = url,
                    IsPrimary = index == 0
                })
                .ToList()
        };

        return await _repository.CreateAsync(product);
    }
}