namespace Product.Api.Requests;

internal sealed record SearchProductsRequest(
    double? MinPrice,
    double? MaxPrice,
    string? Category,
    string? Name,
    int? MinStock
);