using Microsoft.Extensions.Options;
using MongoDB.Driver;
using User.Api.Database.Entities;
using User.Api.Settings;

namespace User.Api.Database;

internal sealed class UserContext
{
    private readonly IMongoDatabase _database;

    public UserContext(IOptions<MongoSettings> mongoSettingsOptions)
    {
        var mongoSettings = mongoSettingsOptions.Value;

        _database = new MongoClient(mongoSettings.ConnectionString)
            .GetDatabase(mongoSettings.Database);
    }

    public IMongoCollection<UserEntity> Users =>
        _database.GetCollection<UserEntity>("Users");
}