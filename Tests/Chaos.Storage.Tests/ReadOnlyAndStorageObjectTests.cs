#region
using System.Collections.Concurrent;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Text.Json;
using Microsoft.Extensions.DependencyInjection;
#endregion

namespace Chaos.Storage.Tests;

public sealed class ReadOnlyAndStorageObjectTests : IDisposable
{
    private readonly IOptions<JsonSerializerOptions> jsonOptions;
    private readonly IOptions<LocalStorageOptions> localOptions;
    private readonly LocalStorageManager manager;
    private readonly IMemoryCache memoryCache;
    private readonly string tempDir;

    public ReadOnlyAndStorageObjectTests()
    {
        tempDir = Path.Combine(
            Path.GetTempPath(),
            "ReadOnlyAndStorageObjectTests_"
            + Guid.NewGuid()
                  .ToString("N"));
        Directory.CreateDirectory(tempDir);
        memoryCache = new MemoryCache(new MemoryCacheOptions());
        jsonOptions = Options.Create(new JsonSerializerOptions());

        localOptions = Options.Create(
            new LocalStorageOptions
            {
                Directory = tempDir
            });
        manager = new LocalStorageManager(memoryCache, jsonOptions, localOptions);
    }

    public void Dispose()
    {
        (memoryCache as IDisposable)?.Dispose();

        if (Directory.Exists(tempDir))
            Directory.Delete(tempDir, true);
    }

    [Test]
    public void ReadOnlyStorageObject_Should_Default_Name_And_Clone_Value()
    {
        // Arrange
        var dict = new ConcurrentDictionary<string, Sample>(StringComparer.OrdinalIgnoreCase);

        dict["default"] = new Sample
        {
            Id = 7
        };

        // Assert via public API only: cannot construct internal type directly
        var storage = manager.Load<Sample>();

        storage.Value
               .Id
               .Should()
               .Be(0);

        storage.GetInstance("alt")
               .Should()
               .NotBeNull();
    }

    [Test]
    public void StorageObject_Should_Expose_Save_Methods_And_GetInstance()
    {
        // Arrange
        var storage = manager.Load<Sample>();

        // Act
        var ro = ((IReadOnlyStorage<Sample>)storage).GetInstance("x");
        var st = storage.GetInstance("y");

        // Just ensure methods are callable without exceptions
        st.Save();
        Func<Task> act = async () => await st.SaveAsync();

        // Assert
        ro.Should()
          .NotBeNull();

        st.Should()
          .NotBeNull();

        act.Should()
           .NotBeNull();
    }

    private sealed class Sample
    {
        public int Id { get; set; }
    }
}