using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace Product.Api.Database.Entities;

[BsonIgnoreExtraElements]
internal sealed class ProductEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; private set; }

    [BsonRequired]
    [BsonElement("code")]
    public string Code { get; init; } = null!;

    [BsonRequired]
    [BsonElement("name")]
    public string Name { get; set; } = null!;

    [BsonRequired]
    [BsonElement("price")]
    public double Price { get; set; }

    [BsonRequired]
    [BsonElement("description")]
    public string Description { get; set; } = null!;

    [BsonRequired]
    [BsonElement("category")]
    public string Category { get; set; } = null!;

    [BsonRequired]
    [BsonElement("stock")]
    public int Stock { get; set; }

    [BsonRequired]
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; set; }

    [BsonRequired]
    [BsonElement("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [BsonElement("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [BsonElement("is_deleted")]
    public bool IsDeleted { get; set; }

    [JsonIgnore]
    [BsonElement("reviews")]
    private IEnumerable<Review> Reviews { get; set; } = [];

    [BsonIgnore]
    public double? AverageRating => Reviews.Any() ? Reviews.Average(r => r.Rating) : null;
}

[BsonIgnoreExtraElements]
public sealed class Review
{
    [BsonRequired]
    [BsonElement("rating")]
    public int Rating { get; private set; }
}