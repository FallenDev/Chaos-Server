using Chaos.Networking.Options;

namespace Chaos.Services.Hosted.Options;

public record WorldOptions : ServerOptions, IWorldOptions
{
    public int DropRange { get; init; }
    public static IWorldOptions Instance { get; set; } = null!;
    public RedirectInfo LoginRedirect { get; init; } = null!;
    public int MaxGoldHeld { get; init; }
    public int PickupRange { get; init; }
    public int RefreshIntervalMs { get; init; }
    public int SaveIntervalMins { get; init; }
    public int TradeRange { get; init; }
    public int UpdatesPerSecond { get; init; }

    public static void PostConfigure(WorldOptions options)
    {
        Instance = options;
        options.LoginRedirect.PopulateAddress();
    }
}