using System.Text;
using System.Text.RegularExpressions;
using Mnemosyne.Domain.Services;

namespace Mnemosyne.Infrastructure.Compression;

/// <summary>
/// Estrategia de compressao que preserva a estrutura do codigo
/// removendo corpos de metodos, comentarios e linhas em branco excessivas.
/// </summary>
public partial class CodeStructureCompressionStrategy : ICompressionStrategy
{
    public string StrategyName => "CodeStructure";

    public Task<CompressionResult> CompressAsync(string content, double targetRatio, CancellationToken cancellationToken = default)
    {
        var compressed = RemoveBlockComments(content);
        compressed = RemoveLineComments(compressed);
        compressed = RemoveXmlDocComments(compressed);
        compressed = RemoveMethodBodies(compressed);
        compressed = RemoveExcessiveBlankLines(compressed);
        compressed = compressed.TrimEnd();

        var actualRatio = content.Length > 0
            ? 1.0 - ((double)compressed.Length / content.Length)
            : 0.0;

        var result = new CompressionResult(
            CompressedContent: compressed,
            OriginalContent: content,
            OriginalLength: content.Length,
            CompressedLength: compressed.Length,
            ActualRatio: Math.Round(actualRatio, 4),
            StrategyUsed: StrategyName);

        return Task.FromResult(result);
    }

    private static string RemoveBlockComments(string content)
    {
        return BlockCommentRegex().Replace(content, string.Empty);
    }

    private static string RemoveLineComments(string content)
    {
        var lines = content.Split('\n');
        var result = new StringBuilder();

        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("//") && !trimmed.StartsWith("///"))
            {
                continue;
            }

            result.AppendLine(line);
        }

        return result.ToString();
    }

    private static string RemoveXmlDocComments(string content)
    {
        var lines = content.Split('\n');
        var result = new StringBuilder();

        foreach (var line in lines)
        {
            var trimmed = line.TrimStart();
            if (trimmed.StartsWith("///"))
            {
                continue;
            }

            result.AppendLine(line);
        }

        return result.ToString();
    }

    private static string RemoveMethodBodies(string content)
    {
        var lines = content.Split('\n');
        var result = new StringBuilder();
        var braceDepth = 0;
        var inMethodBody = false;
        var methodSignatureLine = -1;
        var structuralBraceDepth = 0;

        for (var i = 0; i < lines.Length; i++)
        {
            var trimmed = lines[i].TrimStart();

            if (!inMethodBody)
            {
                if (IsMethodSignature(trimmed))
                {
                    methodSignatureLine = i;
                    result.AppendLine(lines[i]);
                    continue;
                }

                if (methodSignatureLine >= 0 && trimmed == "{")
                {
                    inMethodBody = true;
                    braceDepth = 1;
                    methodSignatureLine = -1;
                    continue;
                }

                methodSignatureLine = -1;

                if (IsStructuralLine(trimmed))
                {
                    if (trimmed == "{")
                        structuralBraceDepth++;
                    else if (trimmed == "}" && structuralBraceDepth > 0)
                        structuralBraceDepth--;

                    result.AppendLine(lines[i]);
                    continue;
                }

                result.AppendLine(lines[i]);
            }
            else
            {
                foreach (var ch in trimmed)
                {
                    if (ch == '{') braceDepth++;
                    else if (ch == '}') braceDepth--;
                }

                if (braceDepth <= 0)
                {
                    inMethodBody = false;
                    braceDepth = 0;
                }
            }
        }

        return result.ToString();
    }

    private static bool IsMethodSignature(string trimmedLine)
    {
        if (string.IsNullOrWhiteSpace(trimmedLine))
            return false;

        if (trimmedLine.StartsWith("using ") ||
            trimmedLine.StartsWith("namespace ") ||
            trimmedLine.StartsWith("public class ") ||
            trimmedLine.StartsWith("public sealed class ") ||
            trimmedLine.StartsWith("public abstract class ") ||
            trimmedLine.StartsWith("internal class ") ||
            trimmedLine.StartsWith("private class ") ||
            trimmedLine.StartsWith("protected class ") ||
            trimmedLine.StartsWith("public interface ") ||
            trimmedLine.StartsWith("internal interface ") ||
            trimmedLine.StartsWith("public enum ") ||
            trimmedLine.StartsWith("internal enum ") ||
            trimmedLine.StartsWith("public record ") ||
            trimmedLine.StartsWith("public record struct ") ||
            trimmedLine.StartsWith("{") ||
            trimmedLine.StartsWith("}"))
            return false;

        if (IsAutoProperty(trimmedLine))
            return false;

        if (IsEnumMember(trimmedLine))
            return false;

        return MethodSignatureRegex().IsMatch(trimmedLine);
    }

    private static bool IsAutoProperty(string trimmedLine)
    {
        return AutoPropertyRegex().IsMatch(trimmedLine);
    }

    private static bool IsEnumMember(string trimmedLine)
    {
        return EnumMemberRegex().IsMatch(trimmedLine);
    }

    private static bool IsStructuralLine(string trimmedLine)
    {
        return trimmedLine == "{" ||
               trimmedLine == "}" ||
               trimmedLine.StartsWith("namespace ") ||
               trimmedLine.StartsWith("public class ") ||
               trimmedLine.StartsWith("public sealed class ") ||
               trimmedLine.StartsWith("public abstract class ") ||
               trimmedLine.StartsWith("internal class ") ||
               trimmedLine.StartsWith("public interface ") ||
               trimmedLine.StartsWith("internal interface ") ||
               trimmedLine.StartsWith("public enum ") ||
               trimmedLine.StartsWith("internal enum ") ||
               trimmedLine.StartsWith("public record ") ||
               trimmedLine.StartsWith("public record struct ");
    }

    private static string RemoveExcessiveBlankLines(string content)
    {
        return ExcessiveBlankLinesRegex().Replace(content, "\n\n");
    }

    [GeneratedRegex(@"/\*.*?\*/", RegexOptions.Singleline)]
    private static partial Regex BlockCommentRegex();

    [GeneratedRegex(@"(public|private|protected|internal|static|async|override|virtual|abstract|sealed)\s+.*\(.*\)\s*$", RegexOptions.None)]
    private static partial Regex MethodSignatureRegex();

    [GeneratedRegex(@"\{[^{}]*\b(get|set|init)\b[^{}]*\}", RegexOptions.None)]
    private static partial Regex AutoPropertyRegex();

    [GeneratedRegex(@"^\w+\s*[,=]?\s*$", RegexOptions.None)]
    private static partial Regex EnumMemberRegex();

    [GeneratedRegex(@"\n{3,}", RegexOptions.None)]
    private static partial Regex ExcessiveBlankLinesRegex();
}
