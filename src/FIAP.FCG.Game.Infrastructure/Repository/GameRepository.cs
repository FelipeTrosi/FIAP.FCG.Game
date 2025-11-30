using FIAP.FCG.Game.Domain.Entity;
using FIAP.FCG.Game.Infrastructure.Repository.Interfaces;

namespace FIAP.FCG.Game.Infrastructure.Repository
{
    public class GameRepository : EFRepository<GameEntity>, IGameRepository
    {
        public GameRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
