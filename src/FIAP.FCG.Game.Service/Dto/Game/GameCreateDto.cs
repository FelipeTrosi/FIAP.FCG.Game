namespace FIAP.FCG.Game.Service.Dto.Game;

/// <summary>
/// DTO utilizado para cadastrar um novo jogo no sistema.
/// </summary>
public class GameCreateDto
{
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
}
