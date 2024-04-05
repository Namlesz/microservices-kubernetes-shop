using Order.Api.Database.Entities;
using Order.Api.Enums;

namespace Order.Api.Abstract.Repositories;

internal interface IOrderCartRepository
{
    public Task<string?> GetUserInProgressCartId(string userId, CancellationToken ct = default);

    public Task<string> GetAndCreateCart(string userId, CancellationToken ct = default);

    public Task<OrderCartEntity?> GetCart(string cartId, CancellationToken ct = default);

    public Task<List<OrderCartEntity>> GetAllUserCarts(string userId, CancellationToken ct = default);

    public Task<string?> GetNotEmptyCartWithStatus(string userId, OrderStatus status, CancellationToken ct = default);

    public Task<bool> IsProductInCart(string cartId, string productCode, CancellationToken ct = default);

    public Task<OrderCartEntity?> AddProductToCart(
        string cartId,
        string productCode,
        int quantity,
        CancellationToken ct = default
    );

    public Task<OrderCartEntity?> RemoveProductFromCart(string cartId, string productCode, CancellationToken ct = default);

    public Task<OrderCartEntity?> PlaceOrder(string cartId, CancellationToken ct = default);

    public Task<OrderCartEntity?> CancelOrder(string cartId, CancellationToken ct = default);
}