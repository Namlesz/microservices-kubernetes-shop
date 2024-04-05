using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Rating.Api.Database.Entities;
using Rating.Api.Settings;

namespace Rating.Api.Database;

internal sealed class RatingContext
{
    private readonly IMongoDatabase _database;

    public RatingContext(IOptions<MongoSettings> mongoSettingsOptions)
    {
        var mongoSettings = mongoSettingsOptions.Value;

        _database = new MongoClient(mongoSettings.ConnectionString)
            .GetDatabase(mongoSettings.Database);
    }

    public IMongoCollection<RatingEntity> Ratings =>
        _database.GetCollection<RatingEntity>("Products");
}