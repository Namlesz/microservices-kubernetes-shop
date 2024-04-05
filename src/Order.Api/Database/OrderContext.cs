using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Order.Api.Database.Entities;
using Order.Api.Settings;

namespace Order.Api.Database;

internal sealed class OrderContext
{
    private readonly IMongoDatabase _database;

    public OrderContext(IOptions<MongoSettings> mongoSettingsOptions)
    {
        var mongoSettings = mongoSettingsOptions.Value;

        _database = new MongoClient(mongoSettings.ConnectionString)
            .GetDatabase(mongoSettings.Database);
    }

    public IMongoCollection<OrderCartEntity> Orders =>
        _database.GetCollection<OrderCartEntity>("Orders");
}