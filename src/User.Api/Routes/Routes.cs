using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using User.Api.Abstract.Repositories;
using User.Api.Abstract.Services;
using User.Api.Database.Entities;
using User.Api.Models;
using User.Api.Requests;
using User.Api.Settings;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace User.Api.Routes;

internal static class Routes
{
    public static void MapRoutes(this WebApplication app)
    {
        app.MapGet("/test-config", ([FromServices] IOptions<JwtTokenSettings> settings) => settings.Value);

        app.MapPost("/register", HandleRegisterUserAsync)
            .WithOpenApi()
            .Produces(Status204NoContent)
            .ProducesProblem(Status409Conflict)
            .ProducesProblem(Status500InternalServerError);

        app.MapPost("/login", HandleLoginUserAsync)
            .WithOpenApi()
            .Produces<TokenDto>()
            .ProducesProblem(Status403Forbidden);

        app.MapPost("/notify", HandleNotifyAsync)
            .WithOpenApi()
            .Produces(Status204NoContent)
            .ProducesProblem(Status404NotFound)
            .ProducesProblem(Status500InternalServerError);
    }

    private static async Task<IResult> HandleRegisterUserAsync(
        [FromBody] RegisterUserRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] ILogger<WebApplication> logger,
        CancellationToken ct
    )
    {
        if (await repository.GetUserAsync(request.Email, ct) is not null)
            return TypedResults.Problem(statusCode: Status409Conflict, detail: "User already exists");

        var hash = BCrypt.Net.BCrypt.HashPassword(request.Password);
        if (hash is null)
        {
            logger.LogError("Failed to hash password");
            return TypedResults.Problem(statusCode: Status500InternalServerError, detail: "Failed to hash password");
        }

        var user = new UserEntity { PasswordHash = hash, Email = request.Email };
        if (!await repository.RegisterUserAsync(user, ct))
        {
            logger.LogError("Failed to save user");
            return TypedResults.Problem(statusCode: Status500InternalServerError,
                detail: "An error occurred while registering the user");
        }

        return TypedResults.NoContent();
    }

    private static async Task<IResult> HandleLoginUserAsync(
        [FromBody] LoginUserRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] ITokenService tokenService,
        CancellationToken ct
    )
    {
        var user = await repository.GetUserAsync(request.Email, ct);
        if (user is null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
            return TypedResults.Problem(statusCode: Status403Forbidden, detail: "Invalid login or password");

        var token = tokenService.GenerateJwtToken(user, ct);
        return TypedResults.Ok(new TokenDto(token));
    }

    private static async Task<IResult> HandleNotifyAsync(
        [FromBody] NotifyRequest request,
        [FromServices] IUserRepository repository,
        [FromServices] IMailService mailService,
        CancellationToken ct
    )
    {
        var user = await repository.GetUserByIdAsync(request.UserId, ct);
        if (user is null)
            return TypedResults.Problem(statusCode: Status404NotFound, detail: "User not found");

        try
        {
            await mailService.SendMailAsync(user.Email, request.Subject, request.Body, ct);
            return TypedResults.NoContent();
        }
        catch (Exception ex)
        {
            return TypedResults.Problem(statusCode: Status500InternalServerError, detail: ex.Message);
        }
    }
}