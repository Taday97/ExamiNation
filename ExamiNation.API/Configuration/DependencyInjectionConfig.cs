using ExamiNation.Application;
using ExamiNation.Infrastructure.Helpers;
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

            var applicationAssembly = Assembly.Load("ExamiNation.Application");
            services.AddServicesFromAssembly(applicationAssembly);

            var infrastructureAssembly = Assembly.Load("ExamiNation.Infrastructure");
            services.AddServicesFromAssembly(infrastructureAssembly);

            services.AddScoped<JwtService>();

            return services;
        }
    }
}