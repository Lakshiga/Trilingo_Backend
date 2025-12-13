using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using TES_Learning_App.Application_Layer.Interfaces.IRepositories;
using TES_Learning_App.Application_Layer.Interfaces.IServices;
using TES_Learning_App.Application_Layer.Services;
namespace TES_Learning_App.Application_Layer
{
    public static class ApplicationServiceExtension
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            // 1. Register Business Logic Services
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IStudentService, StudentService>();
            services.AddScoped<ILanguageService, LanguageService>();
            services.AddScoped<ILevelService, LevelService>();
            services.AddScoped<IStageService, StageService>();
            services.AddScoped<IMainActivityService, MainActivityService>();
            services.AddScoped<IActivityTypeService, ActivityTypeService>();
            services.AddScoped<IActivityService, ActivityService>();
            services.AddScoped<IExerciseService, ExerciseService>();
            services.AddScoped<IRoleService, RoleService>();
            services.AddScoped<IStudentProgressService, StudentProgressService>();
            services.AddScoped<IChatbotService, ChatbotService>();
            // Add other services here later (e.g., IStudentService)

            // 2. Register AutoMapper (we will add this later)
            // services.AddAutoMapper(Assembly.GetExecutingAssembly());

            // 3. Register FluentValidation
            //services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

            return services;
        }
    }
}