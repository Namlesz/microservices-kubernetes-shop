using Microsoft.AspNetCore.Mvc;
using Rating.Api.Abstract.Repositories;
using Rating.Api.Database.Entities;
using Rating.Api.Requests;

namespace Rating.Api.Routes;

internal static class Routes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapPost("/{productCode}/review", HandleAddReviewAsync)
           .WithOpenApi()
           .Produces<RatingEntity>()
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status500InternalServerError)
           .RequireAuthorization();

        app.MapDelete("/{productCode}/review", HandleDeleteReviewAsync)
           .WithOpenApi()
           .RequireAuthorization()
           .Produces(StatusCodes.Status200OK)
           .ProducesProblem(StatusCodes.Status400BadRequest)
           .ProducesProblem(StatusCodes.Status403Forbidden)
           .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("/{productCode}/review", HandleGetReviewsAsync)
           .WithOpenApi()
           .Produces<RatingEntity>()
           .ProducesProblem(StatusCodes.Status404NotFound);
    }

    private static async Task<IResult> HandleAddReviewAsync(
        string productCode,
        ProductReviewRequest request,
        [FromServices] IRatingRepository repository,
        HttpContext context,
        CancellationToken ct)
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        if (await repository.IsUserReviewedAsync(productCode, userId, ct))
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "User already reviewed the product");

        var review = await repository.AddReviewAsync(productCode, userId, request.Rating, request.Comment, ct);
        return review is not null ? TypedResults.Ok(review) : TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Failed to add review");
    }

    private static async Task<IResult> HandleDeleteReviewAsync(
        string productCode,
        [FromServices] IRatingRepository repository,
        HttpContext context,
        CancellationToken ct)
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        if (!await repository.IsUserReviewedAsync(productCode, userId, ct))
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "User did not review the product");

        var isDeleted = await repository.RemoveReviewAsync(productCode, userId, ct);
        return isDeleted ? TypedResults.Ok() : TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Failed to remove review");
    }

    private static async Task<IResult> HandleGetReviewsAsync(
        string productCode,
        [FromServices] IRatingRepository repository,
        CancellationToken ct)
    {
        var reviews = await repository.GetReviewsAsync(productCode, ct);
        return reviews is null ? TypedResults.NotFound() : TypedResults.Ok(reviews);
    }

    private static string? GetUserIdClaim(this HttpContext context)
    {
        return context.User.Claims
            .Where(x => x.Type == "UserId")
            .Select(x => x.Value)
            .FirstOrDefault();
    }
}
