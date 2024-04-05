using Microsoft.AspNetCore.Mvc;
using Order.Api.Abstract.Repositories;
using Order.Api.Abstract.Services;
using Order.Api.Database.Entities;
using Order.Api.Enums;

namespace Order.Api.Routes;

internal static class Routes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapPost("/cart/{productCode}", HandleAddProductToCartAsync)
            .RequireAuthorization()
            .Produces<OrderCartEntity>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapDelete("/cart/{productCode}", HandleRemoveProductFromCartAsync)
            .RequireAuthorization()
            .Produces<OrderCartEntity>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("/cart/submit", HandleSubmitCartAsync)
            .RequireAuthorization()
            .Produces<OrderCartEntity>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapPost("/cart/cancel", HandleCancelCartAsync)
            .RequireAuthorization()
            .Produces<OrderCartEntity>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        app.MapGet("/cart", HandleGetCartsIdsAsync)
            .RequireAuthorization()
            .Produces<List<OrderCartEntity>>()
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status403Forbidden)
            .ProducesProblem(StatusCodes.Status500InternalServerError);

        // TODO: Add product details to the response (?)
        app.MapGet("/cart/{cartId}", HandleGetCartByIdAsync)
            .RequireAuthorization()
            .Produces<OrderCartEntity>()
            .ProducesProblem(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAddProductToCartAsync(
        string productCode,
        [FromQuery] int? quantity,
        [FromServices] IOrderCartRepository orderCartRepository,
        HttpContext context,
        CancellationToken ct
    )
    {
        quantity ??= 1;
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        var cartId = await orderCartRepository.GetAndCreateCart(userId, ct);
        var isProductInCart = await orderCartRepository.IsProductInCart(cartId, productCode, ct);
        if (isProductInCart)
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Product is already in the cart");

        var cart = await orderCartRepository.AddProductToCart(cartId, productCode, quantity.Value, ct);
        return cart is null
            ? TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError,
                detail: "Failed to add product to the cart")
            : TypedResults.Ok(cart);
    }

    private static async Task<IResult> HandleRemoveProductFromCartAsync(
        string productCode,
        [FromServices] IOrderCartRepository orderCartRepository,
        HttpContext context,
        CancellationToken ct
    )
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        var cartId = await orderCartRepository.GetAndCreateCart(userId, ct);
        var isProductInCart = await orderCartRepository.IsProductInCart(cartId, productCode, ct);
        if (!isProductInCart)
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Product is not in the cart");

        var cart = await orderCartRepository.RemoveProductFromCart(cartId, productCode, ct);
        return cart is null
            ? TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError,
                detail: "Failed to remove product from the cart")
            : TypedResults.Ok(cart);
    }

    private static async Task<IResult> HandleSubmitCartAsync(
        [FromServices] IOrderCartRepository orderCartRepository,
        [FromServices] INotifyService notifyService,
        HttpContext context,
        CancellationToken ct
    )
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        var cartId = await orderCartRepository.GetNotEmptyCartWithStatus(userId, OrderStatus.New, ct);
        if (cartId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Cart is empty");

        var cart = await orderCartRepository.PlaceOrder(cartId, ct);
        if (cart is null)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Failed to place order");
        }

        await notifyService.SendMailToUserAsync(userId, "Order placed", "Your order has been placed", ct);

        return TypedResults.Ok(cart);
    }

    private static async Task<IResult> HandleCancelCartAsync(
        [FromServices] IOrderCartRepository orderCartRepository,
        [FromServices] INotifyService notifyService,
        HttpContext context,
        CancellationToken ct
    )
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        var cartId = await orderCartRepository.GetUserInProgressCartId(userId, ct);
        if (cartId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "Cart is empty or not in progress");

        var cart = await orderCartRepository.CancelOrder(cartId, ct);
        if (cart is null)
        {
            return TypedResults.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Failed to cancel order");
        }

        await notifyService.SendMailToUserAsync(userId, "Order canceled", "Your order has been canceled", ct);

        return TypedResults.Ok(cart);
    }

    private static async Task<IResult> HandleGetCartByIdAsync(
        string cartId,
        [FromServices] IOrderCartRepository orderCartRepository,
        HttpContext context,
        CancellationToken ct
    ) => TypedResults.Ok(await orderCartRepository.GetCart(cartId, ct));

    private static async Task<IResult> HandleGetCartsIdsAsync(
        [FromServices] IOrderCartRepository orderCartRepository,
        HttpContext context,
        CancellationToken ct
    )
    {
        var userId = context.GetUserIdClaim();
        if (userId is null)
            return TypedResults.Problem(statusCode: StatusCodes.Status403Forbidden, detail: "User is not authenticated");

        var carts = await orderCartRepository.GetAllUserCarts(userId, ct);
        return TypedResults.Ok(carts);
    }

    private static string? GetUserIdClaim(this HttpContext context)
    {
        return context.User.Claims.FirstOrDefault(x => x.Type == "UserId")?.Value;
    }
}