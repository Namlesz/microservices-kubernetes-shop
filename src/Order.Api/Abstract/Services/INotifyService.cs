namespace Order.Api.Abstract.Services;

internal interface INotifyService
{
    public Task<bool> SendMailToUserAsync(string userId, string subject, string message, CancellationToken ct);
}