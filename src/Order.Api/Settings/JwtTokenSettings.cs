using System.ComponentModel.DataAnnotations;

namespace Order.Api.Settings;

internal sealed class JwtTokenSettings
{
    [Required]
    public string Issuer { get; init; } = null!;

    [Required]
    public string Key { get; init; } = null!;
}