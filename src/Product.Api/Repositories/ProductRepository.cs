using MongoDB.Bson;
using MongoDB.Driver;
using Product.Api.Abstract.Repositories;
using Product.Api.Database;
using Product.Api.Database.Entities;
using Product.Api.Requests;

namespace Product.Api.Repositories;

internal sealed class ProductRepository(ProductContext context, ILogger<ProductRepository> logger) : IProductRepository
{
    public async Task<ProductEntity?> SaveProductAsync(ProductEntity product, CancellationToken ct = default)
    {
        try
        {
            await context.Products.InsertOneAsync(product, cancellationToken: ct);
            return product;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while saving product");
            return null;
        }
    }

    public async Task<ProductEntity?> UpdateProductAsync(ProductEntity product, CancellationToken ct = default)
    {
        try
        {
            var updateDefinition = Builders<ProductEntity>.Update
                .Set(x => x.Name, product.Name)
                .Set(x => x.Price, product.Price)
                .Set(x => x.Description, product.Description)
                .Set(x => x.Category, product.Category)
                .Set(x => x.Stock, product.Stock)
                .Set(x => x.UpdatedAt, DateTime.Now);

            var filter = Builders<ProductEntity>.Filter.Eq(x => x.Code, product.Code);
            var updateResult = await context.Products.UpdateOneAsync(filter, updateDefinition, cancellationToken: ct);

            return updateResult.IsAcknowledged == false | updateResult.ModifiedCount == 0
                ? null
                : product;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Error while updating product");
            return null;
        }
    }

    public async Task<ProductEntity?> GetProductAsync(string code, CancellationToken ct = default) =>
        await context.Products.Find(
                x => x.IsDeleted == false
                    && string.Equals(x.Code, code, StringComparison.OrdinalIgnoreCase)
            )
            .FirstOrDefaultAsync(ct);

    public async Task<List<ProductEntity>?> GetProductsAsync(CancellationToken ct = default) =>
        await context.Products.Find(x => x.IsDeleted == false).ToListAsync(ct);

    public async Task<bool> DeleteProductAsync(string code, CancellationToken ct = default)
    {
        var filter = Builders<ProductEntity>.Filter.Eq(x => x.Code, code);
        var updateDefinition = Builders<ProductEntity>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.DeletedAt, DateTime.Now);

        var updateResult = await context.Products.UpdateOneAsync(filter, updateDefinition, cancellationToken: ct);
        return !(updateResult.IsAcknowledged == false | updateResult.ModifiedCount == 0);
    }

    public Task<List<ProductEntity>> SearchProductsAsync(SearchProductsRequest request, CancellationToken ct = default)
    {
        var filter = Builders<ProductEntity>.Filter.Where(x => x.IsDeleted == false);

        if (request.MinPrice.HasValue)
        {
            filter &= Builders<ProductEntity>.Filter.Gte(x => x.Price, request.MinPrice);
        }

        if (request.MaxPrice.HasValue)
        {
            filter &= Builders<ProductEntity>.Filter.Lte(x => x.Price, request.MaxPrice);
        }

        if (!string.IsNullOrWhiteSpace(request.Category))
        {
            filter &= Builders<ProductEntity>.Filter.Eq(x => x.Category, request.Category);
        }

        if (!string.IsNullOrWhiteSpace(request.Name))
        {
            filter &= Builders<ProductEntity>.Filter.Regex(x => x.Name, new BsonRegularExpression(request.Name, "i"));
        }

        if (request.MinStock.HasValue)
        {
            filter &= Builders<ProductEntity>.Filter.Gte(x => x.Stock, request.MinStock);
        }

        return context.Products.Find(filter).ToListAsync(ct);
    }
}