namespace User.Api.Requests;

internal sealed record NotifyRequest(string UserId, string Subject, string Body);