using User.Api.Database.Entities;

namespace User.Api.Abstract.Repositories;

internal interface IUserRepository
{
    public Task<bool> RegisterUserAsync(UserEntity user, CancellationToken ct = default);

    public Task<UserEntity?> GetUserAsync(string email, CancellationToken ct = default);

    public Task<UserEntity?> GetUserByIdAsync(string id, CancellationToken ct = default);
}