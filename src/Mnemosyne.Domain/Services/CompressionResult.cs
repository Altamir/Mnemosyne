namespace Mnemosyne.Domain.Services;

/// <summary>
/// Resultado da compressao de contexto.
/// </summary>
public record CompressionResult(
    string CompressedContent,
    string OriginalContent,
    int OriginalLength,
    int CompressedLength,
    double ActualRatio,
    string StrategyUsed);
