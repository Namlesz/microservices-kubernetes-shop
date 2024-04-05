using HealthChecks.ApplicationStatus.DependencyInjection;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddProblemDetails();

builder.Services.AddHealthChecks()
    .AddApplicationStatus("Application Status");

var app = builder.Build();

app.MapReverseProxy();
app.UseStatusCodePages();

app.MapGet("/", () => "--- Gateway API is running ---");

app.MapHealthChecks("/healthz", new HealthCheckOptions
    {
        Predicate = _ => true,
        ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
    }
);

app.Run();