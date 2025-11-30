using FIAP.FCG.Game.Infrastructure.Logger;
using FIAP.FCG.Game.Service.Interfaces;
using FIAP.FCG.Game.Service.Services;

namespace FIAP.FCG.Game.API.Extensions; 

public static class ServiceDIExtension
{
    public static IServiceCollection AddServiceDI(this IServiceCollection services)
    {
        services.AddTransient(typeof(IBaseLogger<>), typeof(BaseLogger<>));
        services.AddTransient<IGameService, GameService>();

        return services;
    }
}
