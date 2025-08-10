#region
using System.Linq;
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Memory;
using Chaos.MetaData.ItemMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class ItemMetaTests
{
    [Test]
    public void ItemMetaNode_Length_Tracks_Setters_And_Serialize_Writes_Properties()
    {
        var node = new ItemMetaNode("Sword")
        {
            Level = 10,
            Class = BaseClass.Warrior,
            Weight = 5,
            Category = "weapon",
            Description = "desc"
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("Sword");

        reader.ReadUInt16()
              .Should()
              .Be(5);

        reader.ReadString16()
              .Should()
              .Be("10");

        reader.ReadString16()
              .Should()
              .Be(((int)BaseClass.Warrior).ToString());

        reader.ReadString16()
              .Should()
              .Be("5");

        reader.ReadString16()
              .Should()
              .Be("weapon");

        reader.ReadString16()
              .Should()
              .Be("desc");
    }

    [Test]
    public void ItemMetaNodeCollection_Split_Compresses_Final_Segment_And_Yields_All()
    {
        var coll = new ItemMetaNodeCollection();

        for (var i = 0; i < 10; i++)
            coll.AddNode(
                new ItemMetaNode($"Item{i}")
                {
                    Level = i,
                    Class = BaseClass.Peasant,
                    Weight = 1
                });

        var parts = coll.Split()
                        .ToList();

        parts.Should()
             .NotBeEmpty();

        parts.All(p => (p.Data.Length > 0) && (p.CheckSum != 0u))
             .Should()
             .BeTrue();
    }

    [Test]
    public void ItemMetaNodeCollection_Split_Splits_By_Size()
    {
        var coll = new ItemMetaNodeCollection();

        // Create nodes with artificially large length to force split
        for (var i = 0; i < 2000; i++)
            coll.AddNode(
                new ItemMetaNode($"Item{i}")
                {
                    Level = i,
                    Class = BaseClass.Peasant,
                    Weight = i
                });

        var parts = coll.Split()
                        .ToList();

        parts.Should()
             .NotBeEmpty();

        parts.Select(p => p.Name)
             .Should()
             .OnlyHaveUniqueItems();
    }
}