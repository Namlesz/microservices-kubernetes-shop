using System.ComponentModel.DataAnnotations;

namespace User.Api.Settings;

internal sealed class MongoSettings
{
    [Required]
    public string ConnectionString { get; init; } = null!;

    [Required]
    public string Database { get; init; } = null!;
}