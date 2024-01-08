using Application.Interfaces;
using Application.Profiles;
using Application.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class DependencyInjection
{
    public static void AddApplication(this IServiceCollection services)
    {
        services.AddScoped<IPairService, PairService>();

        // Automapper
        services.AddAutoMapper(typeof(AutoMapperProfile));
    }
}
