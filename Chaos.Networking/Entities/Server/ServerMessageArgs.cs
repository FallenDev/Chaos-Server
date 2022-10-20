using Chaos.Common.Definitions;
using Chaos.Packets.Abstractions;

namespace Chaos.Networking.Entities.Server;

public sealed record ServerMessageArgs : ISendArgs
{
    public string Message { get; set; } = null!;
    public ServerMessageType ServerMessageType { get; set; }
}