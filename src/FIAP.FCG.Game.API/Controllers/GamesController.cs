using FIAP.FCG.Game.Service.Dto.Game;
using FIAP.FCG.Game.Service.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FIAP.FCG.Game.API.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class GamesController(IGameService service) : ControllerBase
    {
        private readonly IGameService _service = service;

        /// <summary>
        /// Cria um novo jogo.
        /// </summary>
        /// <param name="input">Dados do jogo a ser criado.</param>
        /// <response code="200">Jogo criado com sucesso.</response>
        /// <response code="400">Dados inválidos.</response>
        [HttpPost]
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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
        [Authorize(Policy = "Admin")]
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
        [Authorize]
        public IActionResult GetById(long id)
        {
            return Ok(_service.GetById(id));
        }

        /// <summary>
        /// Lista todos os jogos.
        /// </summary>
        /// <response code="200">Lista de jogos retornada com sucesso.</response>
        [HttpGet("GetAll")]
        [Authorize]
        public IActionResult GetAll()
        {
            return Ok(_service.GetAll());
        }

    }
}
