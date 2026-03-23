using Mnemosyne.Domain.Enums;
using Mnemosyne.Domain.Services;

namespace Mnemosyne.Application.Features.Compress.CompressContext;

public class CompressContextHandler
{
    private readonly IReadOnlyDictionary<string, ICompressionStrategy> _strategies;

    public CompressContextHandler(IEnumerable<ICompressionStrategy> strategies)
    {
        _strategies = strategies.ToDictionary(s => s.StrategyName, s => s);
    }

    public async Task<CompressionResult> Handle(CompressContextCommand command, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(command.Content))
        {
            throw new ArgumentException("Content cannot be empty", nameof(command.Content));
        }

        if (command.TargetRatio <= 0.0 || command.TargetRatio >= 1.0)
        {
            throw new ArgumentException("TargetRatio must be between 0 (exclusive) and 1 (exclusive)", nameof(command.TargetRatio));
        }

        var strategyName = command.Strategy.ToString();
        if (!_strategies.TryGetValue(strategyName, out var strategy))
        {
            throw new ArgumentException($"Compression strategy '{strategyName}' is not available", nameof(command.Strategy));
        }

        return await strategy.CompressAsync(command.Content, command.TargetRatio, cancellationToken);
    }
}
