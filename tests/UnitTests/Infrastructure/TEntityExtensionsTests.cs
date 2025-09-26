using Infrastructure.Extensions;
using Vanguard.Tests.Infrastructure.Models;

namespace Vanguard.Tests.Infrastructure;

public class TEntityExtensionsTests
{
    [Fact]
    public void GivenInitDoc_WhenCalled_ThenSetsIdAndCreatedAt()
    {
        // Arrange
        var entity = new DummyEntity();

        // Act
        var result = entity.InitEntity();

        // Assert
        Assert.NotEqual(Guid.Empty, result.Id);
        Assert.True(result.CreatedAt <= DateTimeOffset.UtcNow && result.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void GivenInitDocs_WhenCalled_ThenSetsIdAndCreatedAtForAll()
    {
        // Arrange
        var entities = new List<DummyEntity> { new(), new() };

        // Act
        var result = entities.InitEntities().ToList();

        // Assert
        Assert.All(result, e => Assert.NotEqual(Guid.Empty, e.Id));
        Assert.All(result, e => Assert.True(e.CreatedAt <= DateTimeOffset.UtcNow && e.CreatedAt > DateTimeOffset.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void GivenSetDoc_WhenLastUpdatedByIsNull_ThenSetsLastUpdatedByToSystemAndLastUpdatedOn()
    {
        // Arrange
        var entity = new DummyEntity { LastUpdatedBy = null };

        // Act
        var result = entity.SetEntity();

        // Assert
        Assert.Equal("SYSTEM", result.LastUpdatedBy);
        Assert.True(result.LastUpdatedOn <= DateTimeOffset.UtcNow && result.LastUpdatedOn > DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void GivenSetDoc_WhenLastUpdatedByIsNotNull_ThenDoesNotOverwriteLastUpdatedByButSetsLastUpdatedOn()
    {
        // Arrange
        var entity = new DummyEntity { LastUpdatedBy = "USER" };

        // Act
        var result = entity.SetEntity();

        // Assert
        Assert.Equal("USER", result.LastUpdatedBy);
        Assert.True(result.LastUpdatedOn <= DateTimeOffset.UtcNow && result.LastUpdatedOn > DateTimeOffset.UtcNow.AddMinutes(-1));
    }

    [Fact]
    public void GivenSetDocs_WhenLastUpdatedByIsNull_ThenSetsLastUpdatedByToSystemAndLastUpdatedOnForAll()
    {
        // Arrange
        var entities = new List<DummyEntity> { new(), new() };

        // Act
        var result = entities.SetEntities().ToList();

        // Assert
        Assert.All(result, e => Assert.Equal("SYSTEM", e.LastUpdatedBy));
        Assert.All(result, e => Assert.True(e.LastUpdatedOn <= DateTimeOffset.UtcNow && e.LastUpdatedOn > DateTimeOffset.UtcNow.AddMinutes(-1)));
    }

    [Fact]
    public void GivenSetDocs_WhenLastUpdatedByIsNotNull_ThenDoesNotOverwriteLastUpdatedByButSetsLastUpdatedOnForAll()
    {
        // Arrange
        var entities = new List<DummyEntity>
        {
            new() { LastUpdatedBy = "USER1" },
            new() { LastUpdatedBy = "USER2" }
        };

        // Act
        var result = entities.SetEntities().ToList();

        // Assert
        Assert.Equal("USER1", result[0].LastUpdatedBy);
        Assert.Equal("USER2", result[1].LastUpdatedBy);
        Assert.All(result, e => Assert.True(e.LastUpdatedOn <= DateTimeOffset.UtcNow && e.LastUpdatedOn > DateTimeOffset.UtcNow.AddMinutes(-1)));
    }
}