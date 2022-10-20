using Chaos.IO.Memory;
using Chaos.Networking.Entities.Client;
using Chaos.Packets.Abstractions;
using Chaos.Packets.Abstractions.Definitions;

namespace Chaos.Networking.Deserializers;

public sealed record ExitRequestDeserializer : ClientPacketDeserializer<ExitRequestArgs>
{
    public override ClientOpCode ClientOpCode => ClientOpCode.ExitRequest;

    public override ExitRequestArgs Deserialize(ref SpanReader reader)
    {
        var isRequest = reader.ReadBoolean();

        return new ExitRequestArgs(isRequest);
    }
}