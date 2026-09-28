using AutoMapper;
using Shop.Application.DTOs.ProductDTOs;
using Shop.Application.Interfaces.Repository;
using Shop.Application.Interfaces.Services;
using Shop.Domain.Models;

namespace Shop.Application.Services;

public class ProductService(
    IProductRepository _repository,
    IMapper _mapper,
    ICachingService _cache)
    : IProductService
{
    private const string AllProductsCacheKey = "products:all";

    private static string ProductCacheKey(int id)
        => $"product:{id}";

    public async Task<int> CreateAsync(ProductCreateDTO dto)
    {
        var product = _mapper.Map<Product>(dto);

        product.Images = dto.Images
            .Select((url, index) => new ProductImage
            {
                Url = url,
                IsPrimary = index == 0
            })
            .ToList();

        var id = await _repository.CreateAsync(product);


        await _cache.RemoveAsync(AllProductsCacheKey);

        return id;
    }

    public async Task<List<ProductReadDTO>> GetAllAsync()
    {

        var cachedProducts =
            await _cache.GetAsync<List<ProductReadDTO>>(AllProductsCacheKey);

        if (cachedProducts != null)
            return cachedProducts;


        var products = await _repository.GetAllAsync();

        var result = products.Select(x => new ProductReadDTO
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            Price = x.Price,
            StockQty = x.StockQty,
            IsActive = x.IsActive,
            CategoryId = x.CategoryId,
            Images = x.Images.Select(i => i.Url).ToList()
        }).ToList();


        await _cache.SetAsync(
            AllProductsCacheKey,
            result,
            TimeSpan.FromMinutes(15));

        return result;
    }

    public async Task<ProductReadDTO?> GetByIdAsync(int id)
    {
        var cacheKey = ProductCacheKey(id);


        var cachedProduct =
            await _cache.GetAsync<ProductReadDTO>(cacheKey);

        if (cachedProduct != null)
            return cachedProduct;


        var product = await _repository.GetByIdAsync(id);

        if (product == null)
            return null;

        var result = new ProductReadDTO
        {
            Id = product.Id,
            Name = product.Name,
            Description = product.Description,
            Price = product.Price,
            StockQty = product.StockQty,
            IsActive = product.IsActive,
            CategoryId = product.CategoryId,
            Images = product.Images.Select(i => i.Url).ToList()
        };


        await _cache.SetAsync(
            cacheKey,
            result,
            TimeSpan.FromMinutes(15));

        return result;
    }
}ф