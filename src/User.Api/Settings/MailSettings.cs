using System.ComponentModel.DataAnnotations;

namespace User.Api.Settings;

internal sealed class MailSettings
{
    [Required]
    public string Hostname { get; init; } = null!;

    [Range(1, 65535)]
    public int Port { get; init; }

    [Required]
    public string Username { get; init; } = null!;

    [Required]
    public string Password { get; init; } = null!;
}