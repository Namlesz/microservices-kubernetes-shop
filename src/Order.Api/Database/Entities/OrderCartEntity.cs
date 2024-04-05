using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Order.Api.Enums;

namespace Order.Api.Database.Entities;

internal sealed class OrderCartEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string Id { get; private set; } = null!;

    [BsonRequired]
    [BsonElement("user_id")]
    public string UserId { get; set; } = null!;

    [BsonRequired]
    [BsonElement("status")]
    public OrderStatus Status { get; set; }

    [BsonElement("items")]
    public IEnumerable<OrderItem> Items { get; private set; } = [];
}

internal class OrderItem
{
    [BsonRequired]
    [BsonElement("product_code")]
    public string ProductCode { get; set; } = null!;

    [BsonRequired]
    [BsonElement("quantity")]
    public int Quantity { get; set; }
}