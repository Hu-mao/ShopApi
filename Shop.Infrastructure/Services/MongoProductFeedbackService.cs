using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Shop.Application.DTOs.ProductFeedbackDTOs;
using Shop.Application.Interfaces.Services;
using Shop.Infrastructure.Configuration;
using Shop.Infrastructure.Mongo;

namespace Shop.Infrastructure.Services;

public class MongoProductFeedbackService : IProductFeedbackService
{
    private readonly IMongoCollection<ProductFeedbackDocument> _collection;

    public MongoProductFeedbackService(
        IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var client = new MongoClient(settings.ConnectionString);

        var database = client.GetDatabase(settings.DatabaseName);

        _collection = database.GetCollection<ProductFeedbackDocument>(
            "ProductFeedbacks");
    }

    public async Task AddAsync(
        int productId,
        ProductFeedbackCreateDTO dto)
    {
        var document = new ProductFeedbackDocument
        {
            ProductId = productId,
            Type = dto.Type,
            Text = dto.Text,
            Rating = dto.Rating,
            AuthorName = dto.AuthorName,
            CreatedAt = DateTime.UtcNow
        };

        await _collection.InsertOneAsync(document);
    }
}