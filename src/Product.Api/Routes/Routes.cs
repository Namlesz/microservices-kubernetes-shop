using Microsoft.AspNetCore.Mvc;
using Product.Api.Abstract.Repositories;
using Product.Api.Database.Entities;
using Product.Api.Requests;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace Product.Api.Routes;

internal static class Routes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapPost("/", HandleAddProductAsync)
           .WithOpenApi()
           .Produces<ProductEntity>(Status201Created)
           .ProducesProblem(Status409Conflict)
           .ProducesProblem(Status500InternalServerError)
           .WithSummary("Add a new product")
           .RequireAuthorization();

        app.MapGet("/", HandleGetAllProductsAsync)
           .WithOpenApi()
           .Produces<List<ProductEntity>>()
           .WithSummary("Get all products")
           .AllowAnonymous();

        app.MapPut("/", HandleUpdateProductAsync)
           .WithOpenApi()
           .Produces<ProductEntity>()
           .ProducesProblem(Status404NotFound)
           .ProducesProblem(Status500InternalServerError)
           .WithSummary("Update an existing product")
           .RequireAuthorization();

        app.MapDelete("/", HandleDeleteProductAsync)
           .WithOpenApi()
           .Produces(Status204NoContent)
           .ProducesProblem(Status404NotFound)
           .ProducesProblem(Status500InternalServerError)
           .WithSummary("Delete an existing product")
           .RequireAuthorization();

        app.MapGet("/search", HandleSearchProductsAsync)
           .WithOpenApi()
           .Produces<List<ProductEntity>>()
           .WithSummary("Search products by properties")
           .AllowAnonymous();
    }

    private static async Task<IResult> HandleAddProductAsync(
        [FromBody] AddProductRequest request,
        [FromServices] IProductRepository repository,
        CancellationToken ct)
    {
        if (await repository.GetProductAsync(request.Code, ct) is not null)
            return TypedResults.Problem(statusCode: Status409Conflict, detail: "Product with the same code already exists.");

        var product = new ProductEntity
        {
            Code = request.Code,
            Name = request.Name,
            Price = request.Price,
            Description = request.Description,
            Category = request.Category,
            Stock = request.Stock,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            DeletedAt = null,
            IsDeleted = false
        };

        var saveResult = await repository.SaveProductAsync(product, ct);
        return saveResult is not null ? TypedResults.Created("/", saveResult) : TypedResults.Problem(statusCode: Status500InternalServerError, detail: "Cannot save product. Please try again later.");
    }

    private static async Task<IResult> HandleGetAllProductsAsync(
        [FromServices] IProductRepository repository,
        CancellationToken ct) =>
        TypedResults.Ok(await repository.GetProductsAsync(ct));

    private static async Task<IResult> HandleUpdateProductAsync(
        [FromBody] UpdateProductRequest request,
        [FromServices] IProductRepository repository,
        CancellationToken ct)
    {
        var existingProduct = await repository.GetProductAsync(request.Code, ct);
        if (existingProduct is null)
            return TypedResults.Problem(statusCode: Status404NotFound, detail: "Product not found.");

        existingProduct.Name = request.Name;
        existingProduct.Price = request.Price;
        existingProduct.Description = request.Description;
        existingProduct.Category = request.Category;
        existingProduct.Stock = request.Stock;
        existingProduct.UpdatedAt = DateTime.Now;

        var updateResult = await repository.UpdateProductAsync(existingProduct, ct);
        return updateResult is not null ? TypedResults.Ok(updateResult) : TypedResults.Problem(statusCode: Status500InternalServerError, detail: "Cannot update product. Please try again later.");
    }

    private static async Task<IResult> HandleDeleteProductAsync(
        [FromBody] DeleteProductRequest request,
        [FromServices] IProductRepository repository,
        CancellationToken ct)
    {
        if (await repository.GetProductAsync(request.Code, ct) is null)
            return TypedResults.Problem(statusCode: Status404NotFound, detail: "Product not found.");

        var deleteResult = await repository.DeleteProductAsync(request.Code, ct);
        return deleteResult ? TypedResults.NoContent() : TypedResults.Problem(statusCode: Status500InternalServerError, detail: "Cannot delete product. Please try again later.");
    }

    private static async Task<IResult> HandleSearchProductsAsync(
        [FromBody] SearchProductsRequest request,
        [FromServices] IProductRepository repository,
        CancellationToken ct) =>
        TypedResults.Ok(await repository.SearchProductsAsync(request, ct));
}
