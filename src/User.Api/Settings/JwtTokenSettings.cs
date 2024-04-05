using System.ComponentModel.DataAnnotations;

namespace User.Api.Settings;

internal sealed class JwtTokenSettings
{
    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Key { get; init; } = null!;

    [Range(1, int.MaxValue)]
    public int ExpirationInMinutes { get; init; }
}