using Microsoft.EntityFrameworkCore;
using TES_Learning_App.API.Extensions;
using TES_Learning_App.Application_Layer;
using TES_Learning_App.Infrastructure;
using TES_Learning_App.Infrastructure.Data;
using TES_Learning_App.Infrastructure.Data.DbIntializers_Seeds;
using Microsoft.Extensions.FileProviders;
using System.IO;


namespace TES_Learning_App.API

{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configure to listen on all interfaces (0.0.0.0) for EC2 deployment
            // This allows external access to the backend from outside the EC2 instance
            builder.WebHost.UseUrls("http://0.0.0.0:5166");

            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddApplicationServices();


            // NEW DATABASE WIRING CODE
            // Support for both local development and AWS deployment
            var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
            
            // Allow overriding connection string via environment variable for AWS deployment
            var envConnectionString = Environment.GetEnvironmentVariable("DATABASE_CONNECTION_STRING");
            if (!string.IsNullOrEmpty(envConnectionString))
            {
                connectionString = envConnectionString;
            }
            
            // Validate connection string - LocalDB is Windows-only and won't work on Linux
            if (string.IsNullOrEmpty(connectionString))
            {
                throw new InvalidOperationException(
                    "Database connection string is not configured. " +
                    "Please set 'ConnectionStrings:DefaultConnection' in appsettings.json " +
                    "or set the DATABASE_CONNECTION_STRING environment variable."
                );
            }
            
            // Check if using LocalDB in production (Linux/EC2)
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

            // Add services to the container.
            builder.Services.AddControllers();

            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddOpenApi();
            builder.Services.AddEndpointsApiExplorer();

            // Add SignalR for real-time synchronization
            builder.Services.AddSignalR();

            // Register Realtime Broadcast Service
            builder.Services.AddScoped<TES_Learning_App.API.Services.IRealtimeBroadcastService, TES_Learning_App.API.Services.RealtimeBroadcastService>();

            // Add CORS services - Environment-aware configuration
            builder.Services.AddCors(options =>
            {
                // Production policy: CloudFront + Mobile apps
                // Note: Mobile apps (React Native/Expo) don't have CORS restrictions, but we allow CloudFront
                options.AddPolicy("AllowCloudFront", policy =>
                {
                    policy.SetIsOriginAllowed(origin => 
                        origin == "https://d3v81eez8ecmto.cloudfront.net" ||
                        origin == "http://d3v81eez8ecmto.cloudfront.net" ||
                        origin?.StartsWith("exp://") == true || // Expo dev server
                        origin?.StartsWith("http://localhost") == true || // Local development
                        origin?.StartsWith("http://192.168.") == true || // Local network
                        origin?.StartsWith("http://10.0.2.2") == true // Android emulator
                    )
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials(); // Required for SignalR
                });

                // Development policy: Localhost + CloudFront + Mobile
                // Allow all origins for development (mobile apps, web, etc.)
                options.AddPolicy("AllowAll", builder =>
                {
                    builder.SetIsOriginAllowed(_ => true) // Allow all origins in development
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials(); // Required for SignalR
                });
            });

            // Add your custom Authentication Extension for JWT
            builder.Services.AddApiAuthentication(builder.Configuration);

            // 2. Build the application.
            var app = builder.Build();

            // --- DATABASE SEEDING ---
            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                try
                {
                    var context = services.GetRequiredService<ApplicationDbContext>();
                    // This gets our DbContext and calls our Initializer method.
                    DbInitializer.Initialize(context);
                }
                catch (Exception ex)
                {
                    var logger = services.GetRequiredService<ILogger<Program>>();
                    logger.LogError(ex, "An error occurred while seeding the database.");
                }
            }

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.MapOpenApi();
                // Do not force HTTPS in Development. Emulators/physical devices often fail on self-signed certs.
            }
            else
            {
                app.UseHttpsRedirection();
            }

            // Enable CORS - Must be before SignalR and Authentication
            // Use CloudFront policy in production, AllowAll in development
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

            // Enable static file serving for profile images
            app.UseStaticFiles();

            // Create uploads directory if it doesn't exist
            var uploadsPath = Path.Combine(builder.Environment.ContentRootPath, "wwwroot", "uploads");
            if (!Directory.Exists(uploadsPath))
            {
                Directory.CreateDirectory(uploadsPath);
            }

            // Custom static files for uploads
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(uploadsPath),
                 RequestPath = "/uploads"
            });

            // Map SignalR Hub
            app.MapHub<TES_Learning_App.API.Hubs.AdminHub>("/adminhub");

            app.MapControllers();

            app.Run();
        }
    }
}
