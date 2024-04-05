using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace User.Api.Database.Entities;

internal sealed class UserEntity
{
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string? Id { get; set; }

    [BsonElement("password_hash")]
    public string PasswordHash { get; set; } = null!;

    [BsonElement("email")]
    public string Email { get; set; } = null!;
}