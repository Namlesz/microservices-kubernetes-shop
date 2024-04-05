using Rating.Api.Database.Entities;

namespace Rating.Api.Abstract.Repositories;

internal interface IRatingRepository
{
    public Task<RatingEntity?> GetReviewsAsync(string productCode, CancellationToken ct = default);

    public Task<RatingEntity?> AddReviewAsync(
        string productCode,
        string userId,
        int rating,
        string comment,
        CancellationToken cnl = default
    );

    public Task<bool> RemoveReviewAsync(string productCode, string userId, CancellationToken ct = default);

    public Task<bool> IsUserReviewedAsync(string productCode, string userId, CancellationToken ct = default);
}