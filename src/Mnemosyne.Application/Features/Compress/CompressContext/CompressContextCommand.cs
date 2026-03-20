using Mnemosyne.Domain.Enums;

namespace Mnemosyne.Application.Features.Compress.CompressContext;

public record CompressContextCommand(
    string Content,
    CompressionStrategy Strategy = CompressionStrategy.CodeStructure,
    double TargetRatio = 0.7);
