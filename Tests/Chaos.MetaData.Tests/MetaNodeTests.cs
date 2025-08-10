#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.MetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class MetaNodeTests
{
    [Test]
    public void Serialize_Writes_Name_And_Properties()
    {
        var node = new MetaNode("Test");
        node.Properties.Add("A");
        node.Properties.Add("BB");

        var writer = new SpanWriter(Encoding.GetEncoding(949));

        node.Serialize(ref writer);

        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();

        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("Test");

        reader.ReadUInt16()
              .Should()
              .Be(2);

        reader.ReadString16()
              .Should()
              .Be("A");

        reader.ReadString16()
              .Should()
              .Be("BB");
    }
}