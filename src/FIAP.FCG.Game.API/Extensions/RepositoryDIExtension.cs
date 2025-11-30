using FIAP.FCG.Game.Infrastructure.Repository;
using FIAP.FCG.Game.Infrastructure.Repository.Interfaces;

namespace FIAP.FCG.Game.API.Extensions
{
    public static class RepositoryDIExtension
    {
        public static IServiceCollection AddRepositoryDI(this IServiceCollection services)
        {
            services.AddScoped<IGameRepository, GameRepository>();

            return services;
        }
    }
}
