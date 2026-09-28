using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Shop.Infrastructure.Mongo;

public class ProductFeedbackDocument
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; set; } = string.Empty;

    public int ProductId { get; set; }

    public string Type { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public int? Rating { get; set; }

    public string AuthorName { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; }
}