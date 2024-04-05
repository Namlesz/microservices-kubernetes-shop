using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Product.Api.Database.Entities;
using Product.Api.Settings;

namespace Product.Api.Database;

internal sealed class ProductContext
{
    private readonly IMongoDatabase _database;

    public ProductContext(IOptions<MongoSettings> mongoSettingsOptions)
    {
        var mongoSettings = mongoSettingsOptions.Value;

        _database = new MongoClient(mongoSettings.ConnectionString)
            .GetDatabase(mongoSettings.Database);
    }

    public IMongoCollection<ProductEntity> Products =>
        _database.GetCollection<ProductEntity>("Products");
}