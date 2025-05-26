using ExamiNation.Application;
using ExamiNation.Domain.Interfaces.Security;
using ExamiNation.Domain.Interfaces.Test;
using ExamiNation.Infrastructure.Data;
using ExamiNation.Infrastructure.Helpers;
using ExamiNation.Infrastructure.Repositories;
using ExamiNation.Infrastructure.Repositories.Security;
using ExamiNation.Infrastructure.Repositories.Test;
using System.Reflection;

namespace ExamiNation.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        /// <summary>
        /// Automatically registers all services defined in the ExamiNation.Application and ExamiNation.Infrastructure assemblies 
        /// into the dependency injection container.
        /// </summary>
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            
            services.AddScoped<ICognitiveCategoryRepository, CognitiveCategoryRepository>();
            services.AddScoped<IOptionRepository, OptionRepository>();
            services.AddScoped<IAnswerRepository, AnswerRepository>();
            services.AddScoped<IQuestionRepository, QuestionRepository>();
            services.AddScoped<IScoreRangeRepository, ScoreRangeRespository>();
            services.AddScoped<ITestRepository, TestRepository>();
            services.AddScoped<ITestResultRepository, TestResultRepository>();
           
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoleRepository, RoleRepository>();

            var applicationAssembly = Assembly.Load("ExamiNation.Application");
            services.AddServicesFromAssembly(applicationAssembly);

            //var infrastructureAssembly = Assembly.Load("ExamiNation.Infrastructure");
            //services.AddServicesFromAssembly(infrastructureAssembly);

            services.AddScoped<Seeder>();
            services.AddScoped<JwtService>();

            return services;
        }
    }
}