using FIAP.FCG.Game.Service.Dto.Game;
using FIAP.FCG.Game.Service.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Game.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GamesController(IGameService service, IElasticsearchService metricsService) : ControllerBase
    {
        private readonly IGameService _service = service;
        private readonly IElasticsearchService _metricsService = metricsService;

        /// <summary>
        /// Cria um novo jogo.
        /// </summary>
        /// <param name="input">Dados do jogo a ser criado.</param>
        /// <response code="200">Jogo criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        public IActionResult Post([FromBody] GameCreateDto input)
        {
            _service.Create(input);
            return Ok();
        }

        /// <summary>
        /// Atualiza um jogo existente.
        /// </summary>
        /// <param name="input">Dados do jogo para atualização.</param>
        /// <response code="200">Jogo atualizado com sucesso.</response>
        /// <response code="404">Jogo não encontrado.</response>
        [HttpPut]
        public IActionResult Put([FromBody] GameUpdateDto input)
        {
            _service.Update(input);
            return Ok();
        }

        /// <summary>
        /// Remove um jogo pelo ID.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <response code="200">Jogo removido com sucesso.</response>
        /// <response code="404">Jogo não encontrado.</response>
        [HttpDelete("{id:long}")]
        public IActionResult Delete(long id)
        {
            _service.DeleteById(id);
            return Ok();
        }

        /// <summary>
        /// Obtém um jogo pelo ID.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <response code="200">Jogo encontrado.</response>
        /// <response code="404">Jogo não encontrado.</response>
        [HttpGet("GetById/{id:long}")]
        public IActionResult GetById(long id)
        {
            return Ok(_service.GetById(id));
        }

        /// <summary>
        /// Lista todos os jogos.
        /// </summary>
        /// <response code="200">Lista de jogos retornada com sucesso.</response>
        [HttpGet("GetAll")]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

        /// <summary>
        /// Incrementa venda dos jogos.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <response code="200">Jogo atualizado.</response>
        [HttpPut("IncreasePurchaseCount/{id:long}")]
        public IActionResult IncreasePurchaseCount(long id)
        {
            return Ok(_service.IncreasePurchaseCount(id));
        }

        /// <summary>
        /// Incrementa venda dos jogos.
        /// </summary>
        /// <param name="id">ID do jogo.</param>
        /// <param name="rating">Nota do jogo.</param>
        /// <response code="200">Jogo atualizado.</response>
        [HttpPut("IncreasePurchaseCount/{id:long}/{rating:float}")]
        public IActionResult UpdateRating(long id, float rating)
        {
            return Ok(_service.UpdateRating(id, rating));
        }

        /// <summary>
        /// Busca jogos por termo
        /// </summary>
        /// <param name="q">Termo de busca</param>
        /// <param name="size">Quantidade de resultados</param>
        [HttpGet("search")]
        public async Task<ActionResult<List<GameOutputDto>>> Search(
            [FromQuery] string q,
            [FromQuery] int size = 10)
        {
            if (string.IsNullOrWhiteSpace(q))
                return BadRequest("Termo de busca é obrigatório");

            var results = await _metricsService.SearchGamesAsync(q, size);
            return Ok(results);
        }

        /// <summary>
        /// Recomendações baseadas em gêneros do histórico do usuário
        /// </summary>
        /// <param name="genres">Lista de gêneros do histórico</param>
        /// <param name="size">Quantidade de resultados</param>
        [HttpPost("recommendations")]
        public async Task<ActionResult<List<GameOutputDto>>> GetRecommendations(
            [FromBody] List<string> genres,
            [FromQuery] int size = 10)
        {
            if (genres == null || !genres.Any())
                return BadRequest("Lista de gêneros é obrigatória");

            var results = await _metricsService.GetRecommendationsByUserHistoryAsync(genres, size);
            return Ok(results);
        }

        /// <summary>
        /// Top gêneros por quantidade de jogos
        /// </summary>
        /// <param name="top">Quantidade de gêneros</param>
        [HttpGet("top-genres")]
        public async Task<ActionResult<Dictionary<string, long>>> GetTopGenres(
            [FromQuery] int top = 10)
        {
            var results = await _metricsService.GetTopGamesByGenreAsync(top);
            return Ok(results);
        }

        /// <summary>
        /// Top gêneros por vendas totais
        /// </summary>
        /// <param name="top">Quantidade de gêneros</param>
        [HttpGet("top-genres-by-sales")]
        public async Task<ActionResult<Dictionary<string, long>>> GetTopGenresBySales(
            [FromQuery] int top = 10)
        {
            var results = await _metricsService.GetTopGamesBySalesAsync(top);
            return Ok(results);
        }

        /// <summary>
        /// Avaliação média por gênero
        /// </summary>
        [HttpGet("average-rating-by-genre")]
        public async Task<ActionResult<Dictionary<string, double>>> GetAverageRatingByGenre()
        {
            var results = await _metricsService.GetAverageRatingByGenreAsync();
            return Ok(results);
        }

        /// <summary>
        /// Jogos mais recentes
        /// </summary>
        /// <param name="size">Quantidade de resultados</param>
        [HttpGet("recent-games")]
        public async Task<ActionResult<List<GameOutputDto>>> GetRecentGames(
            [FromQuery] int size = 10)
        {
            var results = await _metricsService.GetMostRecentGamesAsync(size);
            return Ok(results);
        }

        /// <summary>
        /// Métricas gerais do catálogo
        /// </summary>
        [HttpGet("general-metrics")]
        public async Task<ActionResult<Dictionary<string, object>>> GetGeneralMetrics()
        {
            var results = await _metricsService.GetGeneralMetricsAsync();
            return Ok(results);
        }

        /// <summary>
        /// Dashboard completo com todas as métricas
        /// </summary>
        [HttpGet("dashboard")]
        public async Task<ActionResult<object>> GetDashboard()
        {
            var generalMetrics = await _metricsService.GetGeneralMetricsAsync();
            var topGenres = await _metricsService.GetTopGamesByGenreAsync(5);
            var topSales = await _metricsService.GetTopGamesBySalesAsync(5);
            var avgRatings = await _metricsService.GetAverageRatingByGenreAsync();
            var recentGames = await _metricsService.GetMostRecentGamesAsync(5);

            return Ok(new
            {
                general = generalMetrics,
                topGenres = topGenres,
                topGenresBySales = topSales,
                averageRatings = avgRatings,
                recentGames = recentGames
            });
        }

    }
}
