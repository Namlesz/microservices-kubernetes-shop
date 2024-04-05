using Order.Api.Abstract.Services;
using System.Text;
using System.Text.Json;

namespace Order.Api.Services;

internal sealed class NotifyService(IHttpClientFactory httpClientFactory, ILogger<NotifyService> logger) : INotifyService
{
    public async Task<bool> SendMailToUserAsync(string userId, string subject, string message, CancellationToken ct)
    {
        var client = httpClientFactory.CreateClient("NotificationService");
        using var request = new StringContent(JsonSerializer.Serialize(new
            {
                UserId = userId,
                Subject = subject,
                Body = message
            }
        ), Encoding.UTF8, "application/json");

        using var response = await client.PostAsync("/notify", request, ct);
        var content = await response.Content.ReadAsStringAsync(ct);

        if (response.IsSuccessStatusCode)
        {
            return true;
        }

        logger.LogError("Failed to send notification: {Content}", content);
        return false;
    }
}