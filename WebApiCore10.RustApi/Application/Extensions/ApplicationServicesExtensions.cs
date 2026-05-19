using WebApiCore10.RustApi.Application.Services.ErrorHandlingServices;

namespace WebApiCore10.RustApi.Application.Extensions
{
    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationRegistrationServices(this IServiceCollection services)
        {
            
            services.AddScoped<IErrorHandlingService, ErrorHandlingService>();

            return services;
        }
    }
}
