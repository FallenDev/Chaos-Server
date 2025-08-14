#region
using System.Collections.Concurrent;
using System.Text.Json;
using Chaos.Common.Utilities;
using Chaos.Storage;
using Chaos.Storage.Abstractions;
using FluentAssertions;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
#endregion

namespace Chaos.Storage.Tests;

public sealed class LocalStorageManagerTests : IDisposable
{
    private readonly IOptions<JsonSerializerOptions> jsonOptions;
    private readonly IOptions<LocalStorageOptions> localOptions;
    private readonly LocalStorageManager manager;
    private readonly IMemoryCache memoryCache;
    private readonly string tempDir;

    public LocalStorageManagerTests()
    {
        tempDir = Path.Combine(
            Path.GetTempPath(),
            "LocalStorageManagerTests_"
            + Guid.NewGuid()
                  .ToString("N"));
        Directory.CreateDirectory(tempDir);

        memoryCache = new MemoryCache(new MemoryCacheOptions());

        jsonOptions = Options.Create(
            new JsonSerializerOptions
            {
                WriteIndented = false
            });

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
    public void Constructor_Should_Create_Directory_When_Missing()
    {
        var dir = Path.Combine(
            Path.GetTempPath(),
            "LocalStorageCtor_"
            + Guid.NewGuid()
                  .ToString("N"));

        Directory.Exists(dir)
                 .Should()
                 .BeFalse();

        using var mem = new MemoryCache(new MemoryCacheOptions());
        var jso = Options.Create(new JsonSerializerOptions());

        var opts = Options.Create(
            new LocalStorageOptions
            {
                Directory = dir
            });

        _ = new LocalStorageManager(mem, jso, opts);

        Directory.Exists(dir)
                 .Should()
                 .BeTrue();
        Directory.Delete(dir, true);
    }

    [Test]
    public void GetOrAddEntry_Should_Create_In_Cache_When_Missing()
    {
        // Act
        var method = typeof(LocalStorageManager).GetMethod(
            "GetOrAddEntry",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)method.Invoke(manager, [])!;

        // Assert
        dict.Should()
            .NotBeNull();

        dict.Should()
            .BeOfType<ConcurrentDictionary<string, Sample>>();
    }

    [Test]
    public async Task GetOrAddEntryAsync_Should_Create_In_Cache_When_Missing()
    {
        // Act
        var method = typeof(LocalStorageManager).GetMethod(
            "GetOrAddEntryAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)await (Task<ConcurrentDictionary<string, Sample>>)method.Invoke(manager, [])!;

        // Assert
        dict.Should()
            .NotBeNull();

        dict.Should()
            .BeOfType<ConcurrentDictionary<string, Sample>>();
    }

    [Test]
    public void Load_Should_Create_StorageObject_And_Read_Existing_File()
    {
        // Arrange
        var filePath = Path.Combine(tempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["default"] = new Sample
            {
                Id = 1,
                Name = "one"
            }
        };
        JsonSerializerEx.Serialize(filePath, initial, jsonOptions.Value);

        // Act
        var storage = manager.Load<Sample>();

        // Assert
        storage.Should()
               .NotBeNull();

        storage.Value
               .Id
               .Should()
               .Be(1);

        storage.Value
               .Name
               .Should()
               .Be("one");
    }

    [Test]
    public async Task LoadAsync_Should_Create_StorageObject_And_Read_Existing_File()
    {
        // Arrange
        var filePath = Path.Combine(tempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["default"] = new Sample
            {
                Id = 2,
                Name = "two"
            }
        };
        await JsonSerializerEx.SerializeAsync(filePath, initial, jsonOptions.Value);

        // Act
        var storage = await manager.LoadAsync<Sample>();

        // Assert
        storage.Should()
               .NotBeNull();

        storage.Value
               .Id
               .Should()
               .Be(2);

        storage.Value
               .Name
               .Should()
               .Be("two");
    }

    [Test]
    public void LoadOrCreateEntry_Should_Load_From_File_When_Exists_Otherwise_Empty()
    {
        // Arrange
        var filePath = Path.Combine(tempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["name"] = new Sample
            {
                Id = 5
            }
        };
        JsonSerializerEx.Serialize(filePath, initial, jsonOptions.Value);

        // Act
        var method = typeof(LocalStorageManager).GetMethod(
            "LoadOrCreateEntry",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)method.Invoke(manager, [])!;

        // Assert
        dict.Should()
            .ContainKey("name");
    }

    [Test]
    public async Task LoadOrCreateEntryAsync_Should_Load_From_File_When_Exists_Otherwise_Empty()
    {
        // Arrange
        var filePath = Path.Combine(tempDir, "Sample.json");

        var initial = new Dictionary<string, Sample>
        {
            ["name2"] = new Sample
            {
                Id = 6
            }
        };
        await JsonSerializerEx.SerializeAsync(filePath, initial, jsonOptions.Value);

        // Act
        var method = typeof(LocalStorageManager).GetMethod(
            "LoadOrCreateEntryAsync",
            System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic)!.MakeGenericMethod(typeof(Sample));
        var dict = (ConcurrentDictionary<string, Sample>)await (Task<ConcurrentDictionary<string, Sample>>)method.Invoke(manager, [])!;

        // Assert
        dict.Should()
            .ContainKey("name2");
    }

    [Test]
    public void Save_Should_Persist_To_File()
    {
        // Arrange
        var storage = manager.Load<Sample>();
        storage.GetInstance("default");

        storage.Value
               .Id
               .Should()
               .Be(0); // default
        ((IStorage<Sample>)storage).GetInstance("default");
        storage.Value.Id = 3;
        storage.Value.Name = "three";

        // Act
        manager.Save(storage);

        // Assert
        var path = Path.Combine(tempDir, "Sample.json");

        File.Exists(path)
            .Should()
            .BeTrue();
        var data = JsonSerializerEx.Deserialize<Dictionary<string, Sample>>(path, jsonOptions.Value)!;

        data["default"]
            .Id
            .Should()
            .Be(3);

        data["default"]
            .Name
            .Should()
            .Be("three");
    }

    [Test]
    public async Task SaveAsync_Should_Persist_To_File()
    {
        // Arrange
        var storage = await manager.LoadAsync<Sample>();
        storage.Value.Id = 4;
        storage.Value.Name = "four";

        // Act
        await manager.SaveAsync(storage);

        // Assert
        var path = Path.Combine(tempDir, "Sample.json");

        File.Exists(path)
            .Should()
            .BeTrue();
        var data = await JsonSerializerEx.DeserializeAsync<Dictionary<string, Sample>>(path, jsonOptions.Value);

        data!["default"]
            .Id
            .Should()
            .Be(4);

        data!["default"]
            .Name
            .Should()
            .Be("four");
    }

    private sealed class Sample
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}