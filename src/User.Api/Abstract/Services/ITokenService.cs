using User.Api.Database.Entities;

namespace User.Api.Abstract.Services;

internal interface ITokenService
{
    public string GenerateJwtToken(UserEntity user, CancellationToken ct = default);
}