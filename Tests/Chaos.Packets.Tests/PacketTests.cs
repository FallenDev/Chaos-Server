#region
using System.Text;
using FluentAssertions;
#endregion

namespace Chaos.Packets.Tests;

public sealed class PacketTests
{
    [Test]
    public void Ctor_FromOpcode_Sets_Defaults()
    {
        var pkt = new Packet(0x33);

        pkt.OpCode
           .Should()
           .Be(0x33);

        pkt.Signature
           .Should()
           .Be(170);

        pkt.Sequence
           .Should()
           .Be(0);

        pkt.IsEncrypted
           .Should()
           .BeFalse();

        pkt.Buffer
           .Length
           .Should()
           .Be(0);
    }

    [Test]
    public void Ctor_FromSpan_Sets_Header_And_Buffer_Correctly_Encrypted()
    {
        var full = new byte[]
        {
            0xAA,
            0x00,
            0x06,
            0x20,
            0x7E,
            0x41,
            0x42
        }; // len differs but only tail matters
        var span = new Span<byte>(full);
        var pkt = new Packet(ref span, true);

        pkt.IsEncrypted
           .Should()
           .BeTrue();

        pkt.Buffer
           .ToArray()
           .Should()
           .Equal(0x41, 0x42);
    }

    [Test]
    public void Ctor_FromSpan_Sets_Header_And_Buffer_Correctly_Unencrypted()
    {
        var full = new byte[]
        {
            0xAA,
            0x00,
            0x05,
            0x10,
            0x7F,
            0x41,
            0x42
        }; // Sig, len(5), opcode, seq, 'A','B'
        var span = new Span<byte>(full);
        var pkt = new Packet(ref span, false);

        pkt.Signature
           .Should()
           .Be(0xAA);

        pkt.OpCode
           .Should()
           .Be(0x10);

        pkt.Sequence
           .Should()
           .Be(0x7F);

        pkt.IsEncrypted
           .Should()
           .BeFalse();

        pkt.Buffer
           .ToArray()
           .Should()
           .Equal(0x7F, 0x41, 0x42);
    }

    [Test]
    public void GetAsciiString_Replaces_Newlines_When_Requested()
    {
        var data = Encoding.ASCII.GetBytes("A\nB\rC");

        var pkt = new Packet(0x10)
        {
            Buffer = data
        };

        pkt.GetAsciiString()
           .Should()
           .Be("A B C");

        pkt.GetAsciiString(false)
           .Should()
           .Be("A\nB\rC");
    }

    [Test]
    public void GetHexString_Formats_As_Expected()
    {
        var data = new byte[]
        {
            0xDE,
            0xAD,
            0xBE,
            0xEF
        };

        var pkt = new Packet(0x99)
        {
            Buffer = data
        };

        pkt.GetHexString()
           .Should()
           .Be("153: DE AD BE EF");
    }

    [Test]
    public void ToArray_Returns_Copy_Of_Span()
    {
        var pkt = new Packet(0x11)
        {
            Buffer = new byte[]
            {
                7,
                8
            }
        };

        pkt.ToArray()
           .Should()
           .Equal(
               pkt.ToSpan()
                  .ToArray());
    }

    [Test]
    public void ToSpan_And_ToMemory_Include_Header_And_Buffer()
    {
        var payload = new byte[]
        {
            1,
            2,
            3
        };

        var pkt = new Packet(0x44)
        {
            Sequence = 0x02,
            Buffer = payload
        };

        var span = pkt.ToSpan();

        span[0]
            .Should()
            .Be(0xAA);

        span[3]
            .Should()
            .Be(0x44);

        span[^3..]
            .ToArray()
            .Should()
            .Equal(payload);

        var mem = pkt.ToMemory();

        mem.Span[0]
           .Should()
           .Be(0xAA);

        mem.Span[^3..]
           .ToArray()
           .Should()
           .Equal(payload);
    }
}