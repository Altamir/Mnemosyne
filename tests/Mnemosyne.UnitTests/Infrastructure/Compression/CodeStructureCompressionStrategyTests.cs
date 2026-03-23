using Mnemosyne.Infrastructure.Compression;

namespace Mnemosyne.UnitTests.Infrastructure.Compression;

public class CodeStructureCompressionStrategyTests
{
    private readonly CodeStructureCompressionStrategy _strategy;

    public CodeStructureCompressionStrategyTests()
    {
        _strategy = new CodeStructureCompressionStrategy();
    }

    [Fact(DisplayName = "Estratégia retorna nome correto 'CodeStructure'")]
    [Trait("Layer", "Infrastructure - Compression")]
    public void StrategyName_Accessed_ReturnsCodeStructure()
    {
        // Arrange & Act
        var name = _strategy.StrategyName;

        // Assert
        Assert.Equal("CodeStructure", name);
    }

    [Fact(DisplayName = "Compressão remove corpos de métodos mantendo assinaturas")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task MethodBodies_Compressed_KeepsSignaturesOnly()
    {
        // Arrange
        var input = """
            public class Calculator
            {
                public int Add(int a, int b)
                {
                    var result = a + b;
                    return result;
                }

                public int Subtract(int a, int b)
                {
                    return a - b;
                }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("public int Add(int a, int b)", result.CompressedContent);
        Assert.Contains("public int Subtract(int a, int b)", result.CompressedContent);
        Assert.DoesNotContain("var result = a + b", result.CompressedContent);
        Assert.DoesNotContain("return a - b", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão remove comentários de linha")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task LineComments_Compressed_AreRemoved()
    {
        // Arrange
        var input = """
            // This is a comment
            public class Foo
            {
                // Another comment
                public string Name { get; set; }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.DoesNotContain("// This is a comment", result.CompressedContent);
        Assert.DoesNotContain("// Another comment", result.CompressedContent);
        Assert.Contains("public class Foo", result.CompressedContent);
        Assert.Contains("public string Name { get; set; }", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão remove comentários de bloco")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task BlockComments_Compressed_AreRemoved()
    {
        // Arrange
        var input = """
            /* This is a block comment */
            public class Bar
            {
                /*
                 * Multi-line
                 * block comment
                 */
                public int Value { get; set; }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.DoesNotContain("block comment", result.CompressedContent);
        Assert.DoesNotContain("/*", result.CompressedContent);
        Assert.Contains("public class Bar", result.CompressedContent);
        Assert.Contains("public int Value { get; set; }", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão preserva declarações de classe e interface")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task ClassAndInterface_Compressed_ArePreserved()
    {
        // Arrange
        var input = """
            namespace MyApp;

            public interface IService
            {
                Task<string> ProcessAsync(string input);
            }

            public class Service : IService
            {
                public async Task<string> ProcessAsync(string input)
                {
                    await Task.Delay(100);
                    return input.ToUpper();
                }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("public interface IService", result.CompressedContent);
        Assert.Contains("public class Service : IService", result.CompressedContent);
        Assert.Contains("Task<string> ProcessAsync(string input)", result.CompressedContent);
        Assert.Contains("namespace MyApp", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão preserva propriedades auto-implementadas")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task AutoProperties_Compressed_ArePreserved()
    {
        // Arrange
        var input = """
            public class Person
            {
                public string Name { get; set; }
                public int Age { get; private set; }
                public DateTime CreatedAt { get; init; }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("public string Name { get; set; }", result.CompressedContent);
        Assert.Contains("public int Age { get; private set; }", result.CompressedContent);
        Assert.Contains("public DateTime CreatedAt { get; init; }", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão preserva using statements")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task UsingStatements_Compressed_ArePreserved()
    {
        // Arrange
        var input = """
            using System;
            using System.Collections.Generic;

            public class Foo { }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("using System;", result.CompressedContent);
        Assert.Contains("using System.Collections.Generic;", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão retorna metadados corretos no resultado")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task Metadata_Compressed_ReturnsCorrectValues()
    {
        // Arrange
        var input = """
            public class Example
            {
                public void DoWork()
                {
                    Console.WriteLine("Hello");
                    Console.WriteLine("World");
                }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Equal(input, result.OriginalContent);
        Assert.Equal(input.Length, result.OriginalLength);
        Assert.Equal(result.CompressedContent.Length, result.CompressedLength);
        Assert.True(result.CompressedLength < result.OriginalLength);
        Assert.Equal("CodeStructure", result.StrategyUsed);
        Assert.True(result.ActualRatio >= 0.0 && result.ActualRatio <= 1.0);
    }

    [Fact(DisplayName = "Compressão remove linhas em branco excessivas")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task ExcessiveBlankLines_Compressed_AreReduced()
    {
        // Arrange
        var input = "public class Foo\n{\n\n\n\n\n    public int X { get; set; }\n\n\n\n\n}";

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.DoesNotContain("\n\n\n", result.CompressedContent);
        Assert.Contains("public class Foo", result.CompressedContent);
        Assert.Contains("public int X { get; set; }", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão preserva enums")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task Enums_Compressed_ArePreserved()
    {
        // Arrange
        var input = """
            public enum Status
            {
                Active,
                Inactive,
                Deleted
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("public enum Status", result.CompressedContent);
        Assert.Contains("Active", result.CompressedContent);
        Assert.Contains("Inactive", result.CompressedContent);
        Assert.Contains("Deleted", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão preserva records")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task Records_Compressed_ArePreserved()
    {
        // Arrange
        var input = """
            public record PersonDto(string Name, int Age);

            public record struct Point(double X, double Y);
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.Contains("public record PersonDto(string Name, int Age)", result.CompressedContent);
        Assert.Contains("public record struct Point(double X, double Y)", result.CompressedContent);
    }

    [Fact(DisplayName = "Compressão remove XML doc comments")]
    [Trait("Layer", "Infrastructure - Compression")]
    public async Task XmlDocComments_Compressed_AreRemoved()
    {
        // Arrange
        var input = """
            /// <summary>
            /// This is a service
            /// </summary>
            public class MyService
            {
                /// <summary>
                /// Does something important
                /// </summary>
                /// <param name="input">The input</param>
                /// <returns>The result</returns>
                public string Process(string input)
                {
                    return input.Trim();
                }
            }
            """;

        // Act
        var result = await _strategy.CompressAsync(input, 0.7, CancellationToken.None);

        // Assert
        Assert.DoesNotContain("/// <summary>", result.CompressedContent);
        Assert.DoesNotContain("/// <param", result.CompressedContent);
        Assert.Contains("public class MyService", result.CompressedContent);
        Assert.Contains("public string Process(string input)", result.CompressedContent);
    }
}
