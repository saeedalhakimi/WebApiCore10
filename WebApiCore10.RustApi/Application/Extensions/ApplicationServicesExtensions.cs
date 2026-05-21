using WebApiCore10.RustApi.Application.Services.AuthHandlingServices;
using WebApiCore10.RustApi.Application.Services.ErrorHandlingServices;

namespace WebApiCore10.RustApi.Application.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationRegistrationServices(this IServiceCollection services)
        {
            
            services.AddScoped<IErrorHandlingService, ErrorHandlingService>();
            services.AddScoped<IAuthHandlingService, AuthHandlingService>();

            return services;
        }
    }
}
