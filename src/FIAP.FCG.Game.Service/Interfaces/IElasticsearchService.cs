using Elastic.Clients.Elasticsearch;
using FIAP.FCG.Game.Service.Dto.Game;

namespace FIAP.FCG.Game.Service.Interfaces;
public interface IElasticsearchService
{
    Task IndexGameAsync(GameOutputDto entity);
    Task<List<GameOutputDto>> SearchGamesAsync(string searchTerm, int size = 10);
    Task<List<GameOutputDto>> GetRecommendationsByUserHistoryAsync(List<string> userGenres, int size = 10);
    Task<Dictionary<string, long>> GetTopGamesByGenreAsync(int top = 10);
    Task<Dictionary<string, long>> GetTopGamesBySalesAsync(int top = 10);
    Task<Dictionary<string, double>> GetAverageRatingByGenreAsync();
    Task<List<GameOutputDto>> GetMostRecentGamesAsync(int size = 10);
    Task<Dictionary<string, object>> GetGeneralMetricsAsync();
}
