namespace FIAP.FCG.Game.Service.Dto.Game
{
    /// <summary>
    /// DTO que representa os dados de saída de um jogo cadastrado no sistema.
    /// </summary>
    public class GameOutputDto
    {
        /// <summary>
        /// Identificador único do jogo.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Data de criação do jogo.
        /// </summary>
        public DateTime CreatedAt { get; set; }

        /// <summary>
        /// Código interno do jogo.
        /// </summary>
        public long Code { get; set; }

        /// <summary>
        /// Nome do jogo.
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// Descrição do jogo.
        /// </summary>
        public string Description { get; set; } = null!;

        /// <summary>
        /// Data de lançamento.
        /// </summary>
        public DateTime ReleaseDate { get; set; }

        /// <summary>
        /// Total de vendas.
        /// </summary>
        public int PurchaseCount { get; set; }

        /// <summary>
        /// Avialiação do jogo.
        /// </summary>
        public float AverageRating { get; set; }

        /// <summary>
        /// Gênero do jogo.
        /// </summary>
        public string Genre { get; set; } = null!;
    }

}
