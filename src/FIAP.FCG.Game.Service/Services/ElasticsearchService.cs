using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.Aggregations;
using Elastic.Clients.Elasticsearch.QueryDsl;
using FIAP.FCG.Game.Infrastructure.Logger;
using FIAP.FCG.Game.Service.Dto.Game;
using FIAP.FCG.Game.Service.Exceptions;
using FIAP.FCG.Game.Service.Interfaces;

namespace FIAP.FCG.Game.Service.Services;

public class ElasticsearchService(IBaseLogger<ElasticsearchService> logger, ElasticsearchClient client) : IElasticsearchService
{
    private readonly IBaseLogger<ElasticsearchService> _logger = logger;
    private readonly ElasticsearchClient _client = client;
    private const string INDEX_NAME = "games";

    public async Task IndexGameAsync(GameOutputDto entity)
    {
        _logger.LogInformation($"Iniciando serviço de indexação do Jogo com Id {entity.Id} no Elasticsearch !");
        var response = await _client.IndexAsync(entity, INDEX_NAME, entity.Id);

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro ao indexar o Jogo com Id {entity.Id} no Elasticsearch !");
            throw new BadRequestException("Ocorreu um erro ao indexar no Elasticsearch", null);
        }
        else
        {
            _logger.LogInformation($"Jogo indexado com sucesso !");
        }
    }

    /// <summary>
    /// Busca jogos por termo (nome ou descrição)
    /// </summary>
    public async Task<List<GameOutputDto>> SearchGamesAsync(string searchTerm, int size = 10)
    {
        _logger.LogInformation($"Buscando jogos com termo: '{searchTerm}', tamanho: {size}");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(size)
            .Query(q => q
                .Bool(b => b
                    .Should(
                        sh => sh.Match(m => m
                            .Field(f => f.Name)
                            .Query(searchTerm)
                            .Boost(2)
                        ),
                        sh => sh.Match(m => m
                            .Field(f => f.Description)
                            .Query(searchTerm)
                        ),
                        sh => sh.Wildcard(w => w
                            .Field(f => f.Name)
                            .Value($"*{searchTerm.ToLower()}*")
                        )
                    )
                    .MinimumShouldMatch(1)
                )
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro na busca: {response.DebugInformation}");
            throw new Exception($"Erro na busca: {response.DebugInformation}");
        }

        _logger.LogInformation($"Busca realizada com sucesso. Total de resultados: {response.Documents.Count}");
        return response.Documents.ToList();
    }

    /// <summary>
    /// Recomenda jogos baseado no histórico de gêneros do usuário
    /// </summary>
    public async Task<List<GameOutputDto>> GetRecommendationsByUserHistoryAsync(
        List<string> userGenres,
        int size = 10)
    {
        _logger.LogInformation($"Buscando recomendações para gêneros: {string.Join(", ", userGenres)}, tamanho: {size}");

        if (!userGenres.Any())
        {
            _logger.LogWarning("Lista de gêneros está vazia. Retornando lista vazia.");
            return new List<GameOutputDto>();
        }

        var response = await _client.SearchAsync<GameOutputDto>(s => s
           .Index(INDEX_NAME)
           .Size(size)
           .Query(q => q
               .Bool(b => b
                   .Should(
                       userGenres.Select(genre =>
                           new Action<QueryDescriptor<GameOutputDto>>(query =>
                               query.Term(t => t.Field("genre.keyword").Value(genre))
                           )
                       ).ToArray()
                   )
                   .MinimumShouldMatch(1)
               )
           )
           .Sort(sort => sort
               .Field(f => f.AverageRating, new FieldSort { Order = SortOrder.Desc})
               .Field(f => f.PurchaseCount, new FieldSort { Order = SortOrder.Desc })
           )
       );


        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro nas recomendações: {response.DebugInformation}");
            throw new Exception($"Erro nas recomendações: {response.DebugInformation}");
        }

        _logger.LogInformation($"Recomendações obtidas com sucesso. Total: {response.Documents.Count}");
        return response.Documents.ToList();
    }

    /// <summary>
    /// Top jogos por gênero (quantidade de jogos por gênero) - busca todos e agrupa em memória
    /// </summary>
    public async Task<Dictionary<string, long>> GetTopGamesByGenreAsync(int top = 10)
    {
        _logger.LogInformation($"Buscando top {top} gêneros por quantidade de jogos");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(10000)
            .Query(q => q.MatchAll(new MatchAllQuery()))
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro na busca: {response.DebugInformation}");
            throw new Exception($"Erro na busca: {response.DebugInformation}");
        }

        var genreGroups = response.Documents
            .GroupBy(g => g.Genre)
            .Select(group => new { Genre = group.Key, Count = (long)group.Count() })
            .OrderByDescending(x => x.Count)
            .Take(top)
            .ToDictionary(x => x.Genre, x => x.Count);

        _logger.LogInformation($"Top gêneros calculado. Total de gêneros: {genreGroups.Count}");
        return genreGroups;
    }

    /// <summary>
    /// Jogos mais populares por vendas - busca todos e agrupa em memória
    /// </summary>
    public async Task<Dictionary<string, long>> GetTopGamesBySalesAsync(int top = 10)
    {
        _logger.LogInformation($"Buscando top {top} gêneros por vendas");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(10000)
            .Query(q => q.MatchAll(new MatchAllQuery()))
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro na busca: {response.DebugInformation}");
            throw new Exception($"Erro na busca: {response.DebugInformation}");
        }

        var salesByGenre = response.Documents
            .GroupBy(g => g.Genre)
            .Select(group => new
            {
                Genre = group.Key,
                TotalSales = (long)group.Sum(g => g.PurchaseCount)
            })
            .OrderByDescending(x => x.TotalSales)
            .Take(top)
            .ToDictionary(x => x.Genre, x => x.TotalSales);

        _logger.LogInformation($"Top gêneros por vendas calculado. Total de gêneros: {salesByGenre.Count}");
        return salesByGenre;
    }

    /// <summary>
    /// Avaliação média por gênero - busca todos e agrupa em memória
    /// </summary>
    public async Task<Dictionary<string, double>> GetAverageRatingByGenreAsync()
    {
        _logger.LogInformation("Calculando avaliação média por gênero");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(10000)
            .Query(q => q.MatchAll(new MatchAllQuery()))
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro na busca: {response.DebugInformation}");
            throw new Exception($"Erro na busca: {response.DebugInformation}");
        }

        var avgRatingByGenre = response.Documents
            .GroupBy(g => g.Genre)
            .Select(group => new
            {
                Genre = group.Key,
                AvgRating = Math.Round(group.Average(g => g.AverageRating), 2)
            })
            .OrderBy(x => x.Genre)
            .ToDictionary(x => x.Genre, x => x.AvgRating);

        _logger.LogInformation($"Avaliação média calculada. Total de gêneros: {avgRatingByGenre.Count}");
        return avgRatingByGenre;
    }

    /// <summary>
    /// Jogos mais recentes
    /// </summary>
    public async Task<List<GameOutputDto>> GetMostRecentGamesAsync(int size = 10)
    {
        _logger.LogInformation($"Buscando {size} jogos mais recentes");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(size)
            .Sort(sort => sort
                .Field(f => f.ReleaseDate, new FieldSort { Order = SortOrder.Desc })
            )
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro na busca: {response.DebugInformation}");
            throw new Exception($"Erro na busca: {response.DebugInformation}");
        }

        _logger.LogInformation($"Jogos mais recentes obtidos com sucesso. Total: {response.Documents.Count}");
        return response.Documents.ToList();
    }

    /// <summary>
    /// Métricas gerais do catálogo - busca todos e calcula em memória
    /// </summary>
    public async Task<Dictionary<string, object>> GetGeneralMetricsAsync()
    {
        _logger.LogInformation("Calculando métricas gerais do catálogo");

        var response = await _client.SearchAsync<GameOutputDto>(s => s
            .Index(INDEX_NAME)
            .Size(10000)
            .Query(q => q.MatchAll(new MatchAllQuery()))
        );

        if (!response.IsValidResponse)
        {
            _logger.LogError($"Erro nas métricas: {response.DebugInformation}");
            throw new Exception($"Erro nas métricas: {response.DebugInformation}");
        }

        var games = response.Documents.ToList();

        var totalGames = games.Count;
        var totalSales = games.Sum(g => (long)g.PurchaseCount);
        var avgRating = games.Any() ? Math.Round(games.Average(g => g.AverageRating), 2) : 0;
        var totalGenres = games.Select(g => g.Genre).Distinct().Count();

        _logger.LogInformation($"Métricas calculadas - Total jogos: {totalGames}, Total vendas: {totalSales}, Média avaliações: {avgRating}, Total gêneros: {totalGenres}");

        return new Dictionary<string, object>
        {
            { "total_games", totalGames },
            { "total_sales", totalSales },
            { "average_rating", avgRating },
            { "total_genres", totalGenres }
        };
    }
}