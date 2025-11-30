namespace FIAP.FCG.Game.Infrastructure.CorrelationId;

public interface ICorrelationIdGenerator
{
    string Get();
    void Set(string correlationId);
}
