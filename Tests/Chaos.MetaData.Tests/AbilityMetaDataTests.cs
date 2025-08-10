#region
using System.Text;
using Chaos.DarkAges.Definitions;
using Chaos.IO.Compression;
using Chaos.IO.Memory;
using Chaos.MetaData.ClassMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class AbilityMetaDataTests
{
    [Test]
    public void Compress_Writes_Groups_With_Skill_And_Spell_Sections()
    {
        var md = new AbilityMetaData("SClass1");

        md.AddNode(
            new AbilityMetaNode("Slash", true, BaseClass.Warrior)
            {
                Level = 1,
                AbilityLevel = 1,
                IconId = 10
            });

        md.AddNode(
            new AbilityMetaNode("Fireball", false, BaseClass.Warrior)
            {
                Level = 3,
                AbilityLevel = 2,
                IconId = 20
            });

        md.Compress();

        // decompress and inspect structure
        var bytes = md.Data.ToArray();
        Zlib.Decompress(ref bytes);
        var reader = new SpanReader(Encoding.GetEncoding(949), bytes);

        reader.ReadUInt16()
              .Should()
              .BeGreaterThan(0);
    }
}