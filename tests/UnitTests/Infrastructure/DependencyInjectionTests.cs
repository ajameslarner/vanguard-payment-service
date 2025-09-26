using Infrastructure;
using Infrastructure.Extensions;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Abstract;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Vanguard.Tests.Infrastructure.Models;

namespace Vanguard.Tests.Infrastructure;

public class DependencyInjectionTests
{
    [Fact]
    public void GivenAddInfrastructure_WhenMainType_ThenRegistersVanguardContextAndRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Database:DataStoreType", "main"),
                new KeyValuePair<string, string?>("Database:Name", "TestDb")
            ])
            .Build();

        // Act
        services.AddInfrastructure(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var dbContext = provider.GetService<DbContext>();
        Assert.IsType<VanguardContext>(dbContext);

        var repo = provider.GetService<IRepository<DummyEntity>>();
        Assert.NotNull(repo);
        Assert.IsType<GenericRepository<DummyEntity>>(repo);
    }

    [Fact]
    public void GivenAddInfrastructure_WhenBackupType_ThenRegistersVanguardBackupContextAndRepository()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Database:DataStoreType", "backup"),
                new KeyValuePair<string, string?>("Database:Name", "TestDb")
            ])
            .Build();

        // Act
        services.AddInfrastructure(config);
        var provider = services.BuildServiceProvider();

        // Assert
        var dbContext = provider.GetService<DbContext>();
        Assert.IsType<VanguardBackupContext>(dbContext);

        var repo = provider.GetService<IRepository<DummyEntity>>();
        Assert.NotNull(repo);
        Assert.IsType<GenericRepository<DummyEntity>>(repo);
    }

    [Fact]
    public void GivenAddInfrastructure_WhenUnsupportedType_ThenThrowsNotSupportedException()
    {
        // Arrange
        var services = new ServiceCollection();
        services.AddLogging();
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(
            [
                new KeyValuePair<string, string?>("Database:DataStoreType", "invalid"),
                new KeyValuePair<string, string?>("Database:Name", "TestDb")
            ])
            .Build();

        // Act & Assert
        Assert.Throws<NotSupportedException>(() => services.AddInfrastructure(config));
    }
}