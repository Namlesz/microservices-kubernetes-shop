using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using User.Api.Abstract.Services;
using User.Api.Database.Entities;
using User.Api.Settings;

namespace User.Api.Services;

internal sealed class TokenService : ITokenService
{
    private readonly byte[] _secret;
    private readonly string _issuer;
    private readonly DateTime _expiration;

    public TokenService(IOptions<JwtTokenSettings> jwtSettingsOptions)
    {
        var jwtSettings = jwtSettingsOptions.Value;

        _secret = Encoding.ASCII.GetBytes(jwtSettings.Key);
        _expiration = DateTime.UtcNow.AddMinutes(jwtSettings.ExpirationInMinutes);
        _issuer = jwtSettings.Issuer;
    }

    public string GenerateJwtToken(UserEntity user, CancellationToken ct = default)
    {
        ct.ThrowIfCancellationRequested();
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Email, user.Email),
                new Claim("UserId", user.Id!)
            }),
            SigningCredentials = new SigningCredentials(
                new SymmetricSecurityKey(_secret),
                SecurityAlgorithms.HmacSha256Signature
            ),
            Expires = _expiration,
            Issuer = _issuer
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);

        return tokenHandler.WriteToken(token);
    }
}