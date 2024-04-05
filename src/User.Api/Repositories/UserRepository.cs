using MongoDB.Driver;
using User.Api.Abstract.Repositories;
using User.Api.Database;
using User.Api.Database.Entities;

namespace User.Api.Repositories;

internal sealed class UserRepository(UserContext context, ILogger<UserRepository> logger) : IUserRepository
{
    private IMongoCollection<UserEntity> Users => context.Users;

    public async Task<bool> RegisterUserAsync(UserEntity user, CancellationToken ct = default)
    {
        try
        {
            await Users.InsertOneAsync(user, cancellationToken: ct);
            return true;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while registering user");
            return false;
        }
    }

    public async Task<UserEntity?> GetUserAsync(string email, CancellationToken ct = default) =>
        await Users.Find(x => x.Email.Equals(email, StringComparison.CurrentCultureIgnoreCase)).FirstOrDefaultAsync(ct);

    public async Task<UserEntity?> GetUserByIdAsync(string id, CancellationToken ct = default) =>
        await Users.Find(x => x.Id == id).FirstOrDefaultAsync(ct);
}