using System;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Agile_Aggregator.Domain.Models;
using Agile_Aggregator.Application.Services;

namespace Agile_Aggregator.API.Extensions
{
    public static class AuthenticationServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, IConfiguration config)
        {
            // Bind settings
            services.Configure<JwtSettings>(config.GetSection("Jwt"));

            // Token service
            services.AddSingleton<IJwtTokenService, JwtTokenService>();

            // Authentication middleware
            var jwt = config.GetSection("Jwt").Get<JwtSettings>()!;
            var keyBytes = Convert.FromBase64String(jwt.Key);
            var securityKey = new SymmetricSecurityKey(keyBytes);

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwt.Issuer,
                        ValidAudience = jwt.Audience,
                        IssuerSigningKey = securityKey
                    };
                });
           services.AddAuthorization(auth =>
            {
                auth.AddPolicy("ApiScope", p =>
                {
                    p.RequireAuthenticatedUser();
                    p.RequireClaim("scope", config["Jwt:Scope"]);
                });
            });
            return services;
        }
    }
}