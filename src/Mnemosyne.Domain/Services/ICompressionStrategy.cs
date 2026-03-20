namespace Mnemosyne.Domain.Services;

/// <summary>
/// Interface para estrategias de compressao de contexto.
/// </summary>
public interface ICompressionStrategy
{
    string StrategyName { get; }
    Task<CompressionResult> CompressAsync(string content, double targetRatio, CancellationToken cancellationToken = default);
}
