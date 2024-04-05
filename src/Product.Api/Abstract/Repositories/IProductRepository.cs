using Product.Api.Database.Entities;
using Product.Api.Requests;

namespace Product.Api.Abstract.Repositories;

internal interface IProductRepository
{
    public Task<ProductEntity?> SaveProductAsync(ProductEntity product, CancellationToken ct = default);

    public Task<List<ProductEntity>?> GetProductsAsync(CancellationToken ct = default);

    public Task<ProductEntity?> GetProductAsync(string code, CancellationToken ct = default);

    public Task<ProductEntity?> UpdateProductAsync(ProductEntity product, CancellationToken ct = default);

    public Task<bool> DeleteProductAsync(string code, CancellationToken ct = default);

    public Task<List<ProductEntity>> SearchProductsAsync(SearchProductsRequest request, CancellationToken ct = default);
}