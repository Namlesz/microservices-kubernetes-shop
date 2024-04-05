using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.OpenApi.Models;
using Product.Api.Abstract.Repositories;
using Product.Api.Config;
using Product.Api.Database;
using Product.Api.Filters;
using Product.Api.Repositories;
using Product.Api.Routes;
using Product.Api.Settings;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(config =>
{
    config.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            In = ParameterLocation.Header,
            Description = "Please provide a valid token",
            Name = "Authorization",
            Type = SecuritySchemeType.Http,
            BearerFormat = "JWT",
            Scheme = "Bearer"
        }
    );
    config.OperationFilter<BasicAuthOperationsFilter>();
});

// IOptions
builder.Services.AddOptionsWithValidateOnStart<MongoSettings>()
    .Bind(configuration.GetSection(nameof(MongoSettings)))
    .ValidateDataAnnotations()
    .ValidateOnStart();

// Authentication
builder.Services.AddAuthenticationWithJwt(configuration);
builder.Services.AddAuthorization();

// Database
builder.Services.AddSingleton<ProductContext>();

// Settings
builder.Services.AddProblemDetails();

// Health checks
var mongoConfig = configuration.GetSection(nameof(MongoSettings)).Get<MongoSettings>();

builder.Services.AddHealthChecks()
    .AddMongoDb(mongoConfig!.ConnectionString, mongoConfig.Database, "MongoDb")
    .AddApplicationStatus("Application Status");

// Repositories
builder.Services.AddSingleton<IProductRepository, ProductRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePages();

app.UseAuthentication();
app.UseAuthorization();

app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

app.MapRoutes();

app.Run();