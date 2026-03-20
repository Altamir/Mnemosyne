using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Enums;

namespace Mnemosyne.UnitTests.Domain;

public class MemoryEntityTests
{
    [Fact]
    public void Create_WithValidContent_ReturnsMemoryEntity()
    {
        var content = "Test memory content";
        var type = MemoryType.Note;

        var memory = MemoryEntity.Create(content, type);

        Assert.NotNull(memory);
        Assert.Equal(content, memory.Content);
        Assert.Equal(type, memory.Type);
        Assert.NotEqual(Guid.Empty, memory.Id);
        Assert.True(memory.CreatedAt <= DateTime.UtcNow);
    }

    [Fact]
    public void Create_WithEmptyContent_ThrowsArgumentException()
    {
        var emptyContent = "";
        var type = MemoryType.Note;

        var exception = Assert.Throws<ArgumentException>(() => MemoryEntity.Create(emptyContent, type));
        
        Assert.Contains("Content", exception.Message);
    }

    [Fact]
    public void Create_WithWhitespaceContent_ThrowsArgumentException()
    {
        var whitespaceContent = "   ";
        var type = MemoryType.Note;

        var exception = Assert.Throws<ArgumentException>(() => MemoryEntity.Create(whitespaceContent, type));
        
        Assert.Contains("Content", exception.Message);
    }

    [Theory]
    [InlineData(MemoryType.Note)]
    [InlineData(MemoryType.Decision)]
    [InlineData(MemoryType.Preference)]
    [InlineData(MemoryType.Context)]
    public void Create_WithAllMemoryTypes_Succeeds(MemoryType type)
    {
        var content = "Test content";

        var memory = MemoryEntity.Create(content, type);

        Assert.Equal(type, memory.Type);
    }
}
