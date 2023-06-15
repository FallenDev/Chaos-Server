using Chaos.Collections;
using Chaos.Extensions.Common;
using Chaos.Extensions.Geometry;
using Chaos.Geometry.Abstractions;
using Chaos.Geometry.EqualityComparers;
using Chaos.Models.Templates;
using Chaos.Models.World;
using Chaos.Services.Factories.Abstractions;
using Chaos.Time.Abstractions;

namespace Chaos.Models.Data;

public sealed class MonsterSpawn : IDeltaUpdatable
{
    public required ICollection<IPoint> BlackList { get; init; }
    public required ICollection<string> ExtraScriptKeys { get; init; } = Array.Empty<string>();
    public required LootTable? LootTable { get; set; }
    public MapInstance MapInstance { get; set; } = null!;
    public required int MaxAmount { get; init; }
    public required int MaxPerSpawn { get; init; }
    public required IMonsterFactory MonsterFactory { get; init; }
    public required MonsterTemplate MonsterTemplate { get; init; }
    public Rectangle? SpawnArea { get; set; }
    public required IIntervalTimer SpawnTimer { get; init; }

    public void FullSpawn()
    {
        while (true)
        {
            var currentCount = MapInstance.GetEntities<Monster>()
                                          .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

            if (currentCount >= MaxAmount)
                break;

            SpawnMonsters();
        }
    }

    private IPoint GenerateSpawnPoint() => MapInstance.Template.Bounds.GetRandomPoint(PointValidator);

    private bool PointValidator(Point point) => (SpawnArea is null || SpawnArea.Contains(point))
                                                && MapInstance.IsWalkable(point, MonsterTemplate.Type)
                                                && !BlackList.Contains(point, PointEqualityComparer.Instance);

    private void SpawnMonsters()
    {
        var currentCount = MapInstance.GetEntities<Monster>()
                                      .Count(obj => obj.Template.TemplateKey.EqualsI(MonsterTemplate.TemplateKey));

        if (currentCount >= MaxAmount)
            return;

        var spawnAmount = Math.Min(MaxAmount - currentCount, MaxPerSpawn);
        var monsters = new List<Monster>();

        for (var i = 0; i < spawnAmount; i++)
        {
            var point = GenerateSpawnPoint();

            var monster = MonsterFactory.Create(
                MonsterTemplate.TemplateKey,
                MapInstance,
                point,
                ExtraScriptKeys);

            monster.LootTable = LootTable;
            monsters.Add(monster);
        }

        MapInstance.AddObjects(monsters);

        foreach (var monster in monsters)
            monster.Script.OnSpawn();
    }

    /// <inheritdoc />
    public void Update(TimeSpan delta)
    {
        SpawnTimer.Update(delta);

        if (SpawnTimer.IntervalElapsed)
            SpawnMonsters();
    }
}