#region
using System.Text;
using Chaos.IO.Memory;
using Chaos.MetaData.EventMetaData;
using FluentAssertions;
#endregion

namespace Chaos.MetaData.Tests;

public sealed class EventMetaTests
{
    [Test]
    public void EventMetaData_Compress_Writes_Count_And_Data()
    {
        var coll = new EventMetaNodeCollection();
        coll.AddNode(new EventMetaNode("Q1", 1));
        coll.AddNode(new EventMetaNode("Q2", 1));

        var parts = coll.Split()
                        .ToList();
        var md = parts.Single();

        md.Data
          .Should()
          .NotBeNull();

        md.CheckSum
          .Should()
          .NotBe(0u);
    }

    [Test]
    public void EventMetaNode_Serialize_Writes_Sequence_Of_Subnodes()
    {
        var node = new EventMetaNode("QuestName", 1)
        {
            Id = "q1",
            QualifyingCircles = "123",
            QualifyingClasses = "012",
            Summary = "Sum",
            Result = "Res",
            PrerequisiteEventId = "p0",
            Rewards = "Gold"
        };

        var writer = new SpanWriter(Encoding.GetEncoding(949));
        node.Serialize(ref writer);
        writer.Flush();

        var buffer = writer.ToSpan()
                           .ToArray();
        var reader = new SpanReader(Encoding.GetEncoding(949), buffer);

        reader.ReadString8()
              .Should()
              .Be("01_start");
        reader.ReadInt16();

        reader.ReadString8()
              .Should()
              .Be("01_title");
    }
}