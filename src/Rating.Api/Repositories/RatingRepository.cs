using MongoDB.Driver;
using Rating.Api.Abstract.Repositories;
using Rating.Api.Database;
using Rating.Api.Database.Entities;

namespace Rating.Api.Repositories;

internal sealed class RatingRepository(RatingContext context) : IRatingRepository
{
    public async Task<bool> IsUserReviewedAsync(string productCode, string userId, CancellationToken ct = default)
    {
        var filter = Builders<RatingEntity>.Filter.Eq(x => x.Code, productCode)
            & Builders<RatingEntity>.Filter.ElemMatch(x => x.Reviews, x => x.UserId == userId);

        return await context.Ratings.Find(filter).AnyAsync(ct);
    }

    public async Task<RatingEntity?> GetUserReviewAsync(string productCode, string userId, CancellationToken ct = default)
    {
        var filter = Builders<RatingEntity>.Filter.Eq(x => x.Code, productCode)
            & Builders<RatingEntity>.Filter.ElemMatch(x => x.Reviews, x => x.UserId == userId);

        return await context.Ratings.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<RatingEntity?> GetReviewsAsync(string productCode, CancellationToken ct = default)
    {
        var filter = Builders<RatingEntity>.Filter.Eq(x => x.Code, productCode);
        return await context.Ratings.Find(filter).FirstOrDefaultAsync(ct);
    }

    public async Task<RatingEntity?> AddReviewAsync(
        string productCode,
        string userId,
        int rating,
        string comment,
        CancellationToken cnl = default
    )
    {
        var review = new Review
        {
            UserId = userId,
            Rating = rating,
            Comment = comment,
            CreatedAt = DateTime.UtcNow
        };

        var filter = Builders<RatingEntity>.Filter.Eq(x => x.Code, productCode);
        var update = Builders<RatingEntity>.Update.Push(x => x.Reviews, review);
        var options = new FindOneAndUpdateOptions<RatingEntity>
        {
            ReturnDocument = ReturnDocument.After
        };

        return await context.Ratings.FindOneAndUpdateAsync(filter, update, options, cnl);
    }

    public async Task<bool> RemoveReviewAsync(string productCode, string userId, CancellationToken ct = default)
    {
        var filter = Builders<RatingEntity>.Filter.Eq(x => x.Code, productCode)
            & Builders<RatingEntity>.Filter.ElemMatch(x => x.Reviews, x => x.UserId == userId);

        var update = Builders<RatingEntity>.Update.PullFilter(x => x.Reviews, x => x.UserId == userId);

        var updateResult = await context.Ratings.UpdateOneAsync(filter, update, cancellationToken: ct);
        return updateResult.IsAcknowledged && updateResult.ModifiedCount >= 1;
    }
}