using Chaos.Clients.Abstractions;
using Chaos.Collections.Common;
using Chaos.Common.Definitions;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Networking.Abstractions;
using Chaos.Objects.World;
using Chaos.Utilities;

namespace Chaos.Commands.Admin;

[Command("shutdown")]
public class ShutdownCommand : ICommand<Aisling>
{
    private readonly IClientRegistry<IWorldClient> ClientRegistry;
    private readonly IServiceProvider ServiceProvider;

    public ShutdownCommand(IServiceProvider serviceProvider, IClientRegistry<IWorldClient> clientRegistry)
    {
        ServiceProvider = serviceProvider;
        ClientRegistry = clientRegistry;
    }

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var initialMessage))
            return default;

        if (!args.TryGetNext<int>(out var mins))
            return default;

        foreach (var client in ClientRegistry)
            client.SendServerMessage(ServerMessageType.AdminMessage, initialMessage);

        ShutdownUtility.BeginShutdown(ServiceProvider, mins);

        return default;
    }
}