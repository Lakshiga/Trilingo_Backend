using Amazon.S3;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TES_Learning_App.Application_Layer.Interfaces.Infrastructure;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Infrastructure.Data;
using TES_Learning_App.Infrastructure.Repositories;
using TES_Learning_App.Infrastructure.Services_external;


namespace TES_Learning_App.Infrastructure
{
    public static class InfrastructureServiceExtension
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Register the DbContext
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            // 2. Register Repositories
            services.AddScoped<IAuthRepository, AuthRepository>();
            services.AddScoped<IStudentProgressRepository, StudentProgressRepository>();
            // We register it as 'Scoped' so that a new Unit of Work is created for each HTTP request.
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            // 3. Register External Services
            services.AddScoped<ITokenService, TokenService>();
            services.AddScoped<IS3Service, S3Service>();
            services.AddSingleton<IAmazonS3>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var region = config["AWS:Region"];
                var accessKey = config["AWS:AccessKeyId"];
                var secretKey = config["AWS:SecretAccessKey"];

                var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region ?? "ap-southeast-1");

                if (!string.IsNullOrEmpty(accessKey) && !string.IsNullOrEmpty(secretKey))
                {
                    return new AmazonS3Client(accessKey, secretKey, regionEndpoint);
                }
                else
                {
                    return new AmazonS3Client(regionEndpoint);
                }
            });
            // Add other services here later (e.g., IEmailService)

            return services;
        }
    }
}