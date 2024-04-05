namespace User.Api.Requests;

internal sealed record LoginUserRequest(string Email, string Password);