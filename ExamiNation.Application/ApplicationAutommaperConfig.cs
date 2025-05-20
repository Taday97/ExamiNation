using AutoMapper;
using ExamiNation.Application.Mapping.Resolvers;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace ExamiNation.Application
{
    public static class ApplicationAutommaperConfig
    {
        public static void AddAutoMapperServices(this IServiceCollection services)
        {
            var assembly = Assembly.GetExecutingAssembly();

            services.AddHttpContextAccessor();

            var resolverInterfaceType = typeof(IValueResolver<,,>);
            var resolverTypes = assembly.GetTypes()
                .Where(t => !t.IsAbstract && !t.IsInterface)
                .Where(t => t.GetInterfaces()
                    .Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == resolverInterfaceType));

            foreach (var resolverType in resolverTypes)
            {
                services.AddScoped(resolverType);
            }

            services.AddAutoMapper(assembly);
        }
    }
}
