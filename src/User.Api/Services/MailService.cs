using Microsoft.Extensions.Options;
using System.Net;
using System.Net.Mail;
using User.Api.Abstract.Services;
using User.Api.Settings;

namespace User.Api.Services;

internal sealed class MailService(IOptions<MailSettings> options) : IMailService
{
    private readonly MailSettings _settings = options.Value;

    public async Task SendMailAsync(string email, string subject, string body, CancellationToken ct)
    {
        using var message = new MailMessage();

        message.From = new MailAddress(_settings.Username);
        message.To.Add(email);
        message.Subject = subject;
        message.Body = body;

        using var client = new SmtpClient(_settings.Hostname, _settings.Port);
        client.Credentials = new NetworkCredential(_settings.Username, _settings.Password);
        client.EnableSsl = true;

        await client.SendMailAsync(message, ct);
    }
}