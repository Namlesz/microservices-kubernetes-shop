using System.ComponentModel.DataAnnotations;

namespace Rating.Api.Settings;

internal sealed class MongoSettings
{
    [Required]
    public string ConnectionString { get; init; } = null!;

    [Required]
    public string Database { get; init; } = null!;
}