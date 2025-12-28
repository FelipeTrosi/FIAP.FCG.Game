using FIAP.FCG.Game.Service.Dto.Game;

namespace FIAP.FCG.Game.Service.Interfaces;
public interface IGameService
{
    ICollection<GameOutputDto> GetAll();
    GameOutputDto? GetById(long id);
    void Create(GameCreateDto entity);
    void Update(GameUpdateDto entity);
    void DeleteById(long id);
    GameOutputDto? IncreasePurchaseCount(long id);
    GameOutputDto? UpdateRating(long id, float rating);
}
