using AutoFixture;
using Mnemosyne.Application.Features.Project.CreateProject;
using Mnemosyne.Domain.Entities;
using Mnemosyne.Domain.Interfaces;
using Moq;

namespace Mnemosyne.UnitTests.Application.Project;

public class CreateProjectHandlerTests
{
    private readonly Mock<IProjectRepository> _repositoryMock;
    private readonly CreateProjectHandler _handler;
    private readonly Fixture _fixture;

    public CreateProjectHandlerTests()
    {
        _repositoryMock = new Mock<IProjectRepository>();
        _fixture = new Fixture();
        _fixture.Behaviors.Remove(new ThrowingRecursionBehavior());
        _fixture.Behaviors.Add(new OmitOnRecursionBehavior());
        _handler = new CreateProjectHandler(_repositoryMock.Object);
    }

    [Fact(DisplayName = "Criar projeto com nome valido retorna projeto criado")]
    [Trait("Layer", "Application - Commands")]
    public async Task ValidName_Executed_ReturnsCreatedProject()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = _fixture.Create<string>();
        var command = new CreateProjectCommand(name, userId);
        var createdProject = ProjectEntity.Create(name, userId);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(name, result.Name);
        Assert.Equal(userId, result.UserId);
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "Criar projeto com nome vazio lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task EmptyName_Executed_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand(string.Empty, userId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Criar projeto com nome whitespace lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task WhitespaceName_Executed_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var command = new CreateProjectCommand("   ", userId);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Criar projeto com userId invalido lanca ArgumentException")]
    [Trait("Layer", "Application - Commands")]
    public async Task InvalidUserId_Executed_ThrowsArgumentException()
    {
        // Arrange
        var command = new CreateProjectCommand("My Project", Guid.Empty);

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _handler.Handle(command, CancellationToken.None));
        _repositoryMock.Verify(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact(DisplayName = "Criar projeto com descricao opcional valida")]
    [Trait("Layer", "Application - Commands")]
    public async Task OptionalDescription_Executed_ReturnsProjectWithDescription()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var name = _fixture.Create<string>();
        var description = _fixture.Create<string>();
        var command = new CreateProjectCommand(name, userId, description);
        var createdProject = ProjectEntity.Create(name, userId, description);
        _repositoryMock
            .Setup(x => x.AddAsync(It.IsAny<ProjectEntity>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdProject);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(description, result.Description);
    }
}