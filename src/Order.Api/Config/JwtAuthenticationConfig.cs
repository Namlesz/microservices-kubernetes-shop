using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Order.Api.Settings;
using System.Text;

namespace Order.Api.Config;

internal static class JwtAuthenticationConfig
{
    public static void AddAuthenticationWithJwt(this IServiceCollection services, IConfiguration configuration)
    {
        // Add IOptions pattern and validate, same fields are used below in bearer settings
        services.AddOptionsWithValidateOnStart<JwtTokenSettings>()
            .Bind(configuration.GetSection(nameof(JwtTokenSettings)))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            }
        ).AddJwtBearer(x =>
        {
            var jwtTokenSettings = services.BuildServiceProvider().GetRequiredService<IOptions<JwtTokenSettings>>().Value;
            x.TokenValidationParameters = new TokenValidationParameters
            {
                // Fields are validated by IOptions pattern
                ValidIssuer = jwtTokenSettings.Issuer,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtTokenSettings.Key)),
                ValidateIssuer = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.Zero
            };
            x.Events = new JwtBearerEvents
            {
                OnChallenge = async context =>
                {
                    context.HandleResponse();

                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                    await context.Response.WriteAsJsonAsync(new ProblemDetails
                    {
                        Status = StatusCodes.Status401Unauthorized,
                        Title = "Unauthorized",
                        Detail = "Missing or invalid token",
                        Instance = context.Request.Path
                    });
                }
            };
        });
    }
}