using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using TES_Learning_App.API.Configuration;

namespace TES_Learning_App.API.Extensions
{
    public static class AuthenticationServiceExtension
    {
        public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
        {
            // Bind the JWT settings from appsettings.json to our JwtConfig class
            var jwtConfig = new JwtConfig();
            configuration.Bind("JwtSettings", jwtConfig);
            
            // Validate JWT configuration
            if (string.IsNullOrWhiteSpace(jwtConfig.Secret))
            {
                throw new InvalidOperationException(
                    "JWT Secret is not configured. Please set 'JwtSettings:Secret' in appsettings.json or environment variables. " +
                    $"Current value: '{jwtConfig.Secret ?? "null"}'"
                );
            }
            
            if (string.IsNullOrWhiteSpace(jwtConfig.Issuer))
            {
                throw new InvalidOperationException(
                    "JWT Issuer is not configured. Please set 'JwtSettings:Issuer' in appsettings.json or environment variables."
                );
            }
            
            if (string.IsNullOrWhiteSpace(jwtConfig.Audience))
            {
                throw new InvalidOperationException(
                    "JWT Audience is not configured. Please set 'JwtSettings:Audience' in appsettings.json or environment variables."
                );
            }
            
            services.AddSingleton(jwtConfig); // Make it available via DI if needed

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.Secret)),
                    ValidateIssuer = true,
                    ValidIssuer = jwtConfig.Issuer,
                    ValidateAudience = true,
                    ValidAudience = jwtConfig.Audience,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };
                
                // Handle [AllowAnonymous] properly by not challenging anonymous endpoints
                options.Events = new JwtBearerEvents
                {
                    OnChallenge = context =>
                    {
                        // Check if the endpoint allows anonymous access
                        var endpoint = context.HttpContext.GetEndpoint();
                        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
                        {
                            // Don't challenge anonymous endpoints
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    },
                    OnAuthenticationFailed = context =>
                    {
                        // Check if the endpoint allows anonymous access
                        var endpoint = context.HttpContext.GetEndpoint();
                        if (endpoint?.Metadata?.GetMetadata<Microsoft.AspNetCore.Authorization.IAllowAnonymous>() != null)
                        {
                            // Don't fail authentication for anonymous endpoints
                            context.NoResult();
                            return Task.CompletedTask;
                        }
                        return Task.CompletedTask;
                    }
                };
            });

            return services;
        }
    }
}