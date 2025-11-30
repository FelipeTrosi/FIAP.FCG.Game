namespace FIAP.FCG.Game.Infrastructure.Logger;

public interface IBaseLogger<T>
{
    void LogInformation(string message);

    void LogError(string message);

    void LogWarning(string message);
}
