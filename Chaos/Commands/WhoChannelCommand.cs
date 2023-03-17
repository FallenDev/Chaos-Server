using Chaos.Collections.Common;
using Chaos.Messaging;
using Chaos.Messaging.Abstractions;
using Chaos.Objects.World;

namespace Chaos.Commands;

[Command("whochannel", false)]
public class WhoChannelCommand : ICommand<Aisling>
{
    private readonly IChannelService ChannelService;

    public WhoChannelCommand(IChannelService channelService) => ChannelService = channelService;

    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        if (!args.TryGetNext<string>(out var channelName))
            return default;

        var subs = ChannelService.GetSubscribers(channelName);

        foreach (var sub in subs)
            source.SendOrangeBarMessage(sub.Name);

        return default;
    }
}