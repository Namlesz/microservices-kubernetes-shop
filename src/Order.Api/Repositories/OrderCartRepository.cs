using MongoDB.Driver;
using Order.Api.Abstract.Repositories;
using Order.Api.Database;
using Order.Api.Database.Entities;
using Order.Api.Enums;

namespace Order.Api.Repositories;

internal sealed class OrderCartRepository(OrderContext context) : IOrderCartRepository
{
    public async Task<string?> GetUserInProgressCartId(string userId, CancellationToken ct = default) =>
        (await context.Orders
            .Find(cart =>
                cart.UserId == userId
                && cart.Status == OrderStatus.InProgress
                | cart.Status == OrderStatus.New
                | cart.Status == OrderStatus.Completed
            )
            .FirstOrDefaultAsync(ct)
        )?.Id;

    public async Task<string> GetAndCreateCart(string userId, CancellationToken ct = default)
    {
        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.UserId, userId)
            & Builders<OrderCartEntity>.Filter.Eq(cart => cart.Status, OrderStatus.New);
        var cart = await context.Orders.Find(filter).FirstOrDefaultAsync(ct);
        if (cart is not null)
        {
            return cart.Id;
        }

        var newCart = new OrderCartEntity
        {
            UserId = userId,
            Status = OrderStatus.New
        };

        await context.Orders.InsertOneAsync(newCart, cancellationToken: ct);
        return newCart.Id;
    }

    public async Task<OrderCartEntity?> GetCart(string cartId, CancellationToken ct = default) =>
        await context.Orders.Find(cart => cart.Id == cartId).FirstOrDefaultAsync(ct);

    public async Task<List<OrderCartEntity>> GetAllUserCarts(
        string userId,
        CancellationToken ct = default
    ) => await context.Orders.Find(cart => cart.UserId == userId).ToListAsync(ct);

    public async Task<string?> GetNotEmptyCartWithStatus(
        string userId,
        OrderStatus status,
        CancellationToken ct = default
    )
    {
        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.UserId, userId)
            & Builders<OrderCartEntity>.Filter.Eq(cart => cart.Status, status)
            & Builders<OrderCartEntity>.Filter.SizeGte(cart => cart.Items, 1);

        return await context.Orders.Find(filter).Project(cart => cart.Id).FirstOrDefaultAsync(ct);
    }

    public async Task<bool> IsProductInCart(
        string cartId,
        string productCode,
        CancellationToken ct = default
    ) =>
        await context.Orders
            .Find(cart => cart.Id == cartId && cart.Items.Any(item => item.ProductCode == productCode))
            .AnyAsync(cancellationToken: ct);

    public async Task<OrderCartEntity?> AddProductToCart(
        string cartId,
        string productCode,
        int quantity,
        CancellationToken ct = default
    )
    {
        var orderItem = new OrderItem
        {
            ProductCode = productCode,
            Quantity = quantity
        };

        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.Status, OrderStatus.New)
            & Builders<OrderCartEntity>.Filter.Eq(cart => cart.Id, cartId);
        var update = Builders<OrderCartEntity>.Update.Push(cart => cart.Items, orderItem);
        var options = new FindOneAndUpdateOptions<OrderCartEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Orders.FindOneAndUpdateAsync(filter, update, options, ct);
    }

    public async Task<OrderCartEntity?> RemoveProductFromCart(
        string cartId,
        string productCode,
        CancellationToken ct = default
    )
    {
        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.Id, cartId)
            & Builders<OrderCartEntity>.Filter.ElemMatch(cart => cart.Items, x => x.ProductCode == productCode)
            & Builders<OrderCartEntity>.Filter.Eq(cart => cart.Status, OrderStatus.New);
        var update = Builders<OrderCartEntity>.Update.PullFilter(cart => cart.Items, item => item.ProductCode == productCode);
        var options = new FindOneAndUpdateOptions<OrderCartEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Orders.FindOneAndUpdateAsync(filter, update, options, ct);
    }

    public async Task<OrderCartEntity?> PlaceOrder(string cartId, CancellationToken ct = default)
    {
        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.Id, cartId)
            & Builders<OrderCartEntity>.Filter.Eq(cart => cart.Status, OrderStatus.New);
        var update = Builders<OrderCartEntity>.Update.Set(cart => cart.Status, OrderStatus.InProgress);
        var options = new FindOneAndUpdateOptions<OrderCartEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Orders.FindOneAndUpdateAsync(filter, update, options, ct);
    }

    public async Task<OrderCartEntity?> CancelOrder(string cartId, CancellationToken ct = default)
    {
        var filter = Builders<OrderCartEntity>.Filter.Eq(cart => cart.Id, cartId);
        var update = Builders<OrderCartEntity>.Update.Set(cart => cart.Status, OrderStatus.Canceled);
        var options = new FindOneAndUpdateOptions<OrderCartEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Orders.FindOneAndUpdateAsync(filter, update, options, ct);
    }
}