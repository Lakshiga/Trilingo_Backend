using Microsoft.EntityFrameworkCore;
using TES_Learning_App.API.Extensions;
using TES_Learning_App.Application_Layer;
using TES_Learning_App.Infrastructure;
using TES_Learning_App.Infrastructure.Data;
using TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds;
using Microsoft.Extensions.FileProviders;
using System.IO;
using System.Linq;


namespace TES_Learning_App.API

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.WebHost.UseUrls("http://0.0.0.0:5166");

            // Add ContentRootPath and Environment to configuration for S3Service
            builder.Configuration["ContentRootPath"] = builder.Environment.ContentRootPath;
            builder.Configuration["ASPNETCORE_ENVIRONMENT"] = builder.Environment.EnvironmentName;

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddApplicationServices();

            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                connectionString = envConnectionString;
            }
            
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured. " +
                    "Please set 'ConnectionStrings:DefaultConnection' in appsettings.json " +
                    "or set the DATABASE_CONNECTION_STRING environment variable."
                );
            }
            
            if (connectionString.Contains("(localdb)", StringComparison.OrdinalIgnoreCase) && 
                builder.Environment.IsProduction())
            {
                throw new InvalidOperationException(
                    "LocalDB is not supported on Linux/EC2. " +
                    "Please set the DATABASE_CONNECTION_STRING environment variable with a proper SQL Server connection string. " +
                    "Example: export DATABASE_CONNECTION_STRING='Server=your-server;Database=your-db;User Id=user;Password=pass;'"
                );
            }
            
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    // Configure JSON to accept both camelCase and PascalCase
                    // This ensures compatibility with frontend sending PascalCase
                    options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
                    // Keep default camelCase for serialization (outgoing)
                    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
                });

            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services.AddSignalR();

            builder.Services.AddScoped<TES_Learning_App.API.Services.IRealtimeBroadcastService, TES_Learning_App.API.Services.RealtimeBroadcastService>();

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowCloudFront", policy =>
                {
                    policy.SetIsOriginAllowed(origin => 
                        origin == "https://d3v81eez8ecmto.cloudfront.net" ||
                        origin == "http://d3v81eez8ecmto.cloudfront.net" ||
                        origin?.StartsWith("exp://") == true || 
                        origin?.StartsWith("http://localhost") == true ||
                        origin?.StartsWith("http://192.168.") == true || 
                        origin?.StartsWith("http://10.0.2.2") == true ||
                        origin == null // Allow null origin for mobile apps making direct requests
                    )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                });

                options.AddPolicy("AllowAll", builder =>
                {
                    builder.SetIsOriginAllowed(_ => true)
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
            });

            builder.Services.AddApiAuthentication(builder.Configuration);

            var app = builder.Build();

            // Apply database migrations automatically on startup
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var logger = services.GetRequiredService<ILogger<Program>>();
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    
                    // Check for pending migrations
                    var pendingMigrations = context.Database.GetPendingMigrations().ToList();
                    if (pendingMigrations.Any())
                    {
                        logger.LogInformation("Applying {Count} pending database migration(s)...", pendingMigrations.Count);
                        foreach (var migration in pendingMigrations)
                        {
                            logger.LogInformation("  - {Migration}", migration);
                        }
                        
                        // Apply all pending migrations
                        context.Database.Migrate();
                        logger.LogInformation("✅ Database migrations applied successfully!");
                    }
                    else
                    {
                        logger.LogInformation("✅ Database is up to date - no pending migrations.");
                    }
                    
                    // Initialize database (seed data)
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex, "❌ An error occurred while applying database migrations or seeding the database.");
                    // Don't throw - allow app to start even if migration fails
                    // This prevents the app from crashing if there's a temporary DB connection issue
                }
            }

            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
            }
            else
            {
                app.UseHttpsRedirection();
            }

            if (app.Environment.IsProduction())
            {
                app.UseCors("AllowCloudFront");
            }
            else
            {
                app.UseCors("AllowAll");
            }

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseStaticFiles();

            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                 RequestPath = "/uploads"
            });

            app.MapHub<TES_Learning_App.API.Hubs.AdminHub>("/adminhub");

            app.MapControllers();

            app.Run();
        }
    }
}
