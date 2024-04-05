using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Rating.Api.Database.Entities;

[BsonIgnoreExtraElements]
internal sealed class RatingEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = null!;

    [BsonRequired]
    [BsonElement("code")]
    public string Code { get; private set; } = null!;

    [BsonElement("reviews")]
    public IEnumerable<Review> Reviews { get; private set; } = [];
}

internal sealed class Review
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = ObjectId.GenerateNewId().ToString();

    [BsonRequired]
    [BsonElement("user_id")]
    public string UserId { get; init; } = null!;

    [BsonRequired]
    [BsonElement("rating")]
    public int Rating { get; init; }

    [BsonRequired]
    [BsonElement("comment")]
    public string Comment { get; init; } = null!;

    [BsonRequired]
    [BsonElement("created_at")]
    public DateTime CreatedAt { get; init; }
}