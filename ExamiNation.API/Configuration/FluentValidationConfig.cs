using FluentValidation.AspNetCore;
using FluentValidation;
using ExamiNation.Application.Validators.Auth;

namespace ExamiNation.API.Configuration
{
    public static class FluentValidationConfig
    {
        /// <summary>
        /// Registers FluentValidation validators and enables both automatic model validation and client-side adapters.
        /// </summary>
        /// <param name="services">The IServiceCollection to add the validation services to.</param>
        public static void AddFluentValidationServices(this IServiceCollection services)
        {

            var domainAssembly = typeof(RegisterModelDtoValidator).Assembly;
            services.AddValidatorsFromAssembly(domainAssembly);

            services.AddFluentValidationAutoValidation();
            services.AddFluentValidationClientsideAdapters();
        }
    }
}
