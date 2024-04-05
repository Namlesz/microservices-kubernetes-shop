using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using User.Api.Abstract.Repositories;
using User.Api.Abstract.Services;
using User.Api.Database;
using User.Api.Repositories;
using User.Api.Routes;
using User.Api.Services;
using User.Api.Settings;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// IOptions
builder.Services.AddOptionsWithValidateOnStart<MongoSettings>()
    .Bind(configuration.GetSection(nameof(MongoSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptionsWithValidateOnStart<JwtTokenSettings>()
    .Bind(configuration.GetSection(nameof(JwtTokenSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptionsWithValidateOnStart<MailSettings>()
    .Bind(configuration.GetSection(nameof(MailSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Database
builder.Services.AddSingleton<UserContext>();

// Settings
builder.Services.AddProblemDetails();

// Services
builder.Services.AddScoped<ITokenService, TokenService>();
builder.Services.AddScoped<IMailService, MailService>();

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Health checks
var mongoConfig = configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

builder.Services.AddHealthChecks()
    .AddMongoDb(mongoConfig!.ConnectionString, mongoConfig.Database, "MongoDb")
    .AddApplicationStatus("Application Status");

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseStatusCodePages();

app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

app.MapRoutes();

app.Run();