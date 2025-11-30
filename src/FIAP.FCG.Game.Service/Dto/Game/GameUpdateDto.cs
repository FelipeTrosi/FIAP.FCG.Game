namespace FIAP.FCG.Game.Service.Dto.Game;

/// <summary>
/// DTO para atualização de um jogo existente.
/// </summary>
public class GameUpdateDto
{
    /// <summary>
    /// Identificador único do jogo a ser atualizado.
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// Data de criação original do jogo.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Código interno ou identificador do jogo.
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