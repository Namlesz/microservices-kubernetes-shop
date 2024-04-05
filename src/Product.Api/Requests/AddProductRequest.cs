using System.ComponentModel.DataAnnotations;

namespace Product.Api.Requests;

internal sealed record AddProductRequest(
    [Required] string Code,
    [Required] string Name,
    [Range(0.01, double.MaxValue)] double Price,
    [Required] string Description,
    [Required] string Category,
    [Range(0, int.MaxValue)] int Stock
);