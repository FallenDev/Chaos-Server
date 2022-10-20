using Chaos.Geometry;
using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record PickupDeserializer : ClientPacketDeserializer<PickupArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.Pickup;

    public override PickupArgs Deserialize(ref SpanReader reader)
    {
        var destinationSlot = reader.ReadByte();
        Point sourcePoint = reader.ReadPoint16();

        return new PickupArgs(destinationSlot, sourcePoint);
    }
}