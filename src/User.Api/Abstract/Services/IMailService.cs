namespace User.Api.Abstract.Services;

internal interface IMailService
{
    public Task SendMailAsync(string email, string subject, string body, CancellationToken ct);
}