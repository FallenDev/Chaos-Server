#region
using System.Diagnostics;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     Provides ActivitySources for OpenTelemetry tracing throughout the Chaos server.
/// </summary>
public static class ChaosActivitySource
{
    /// <summary>
    ///     The name of the general activity source.
    /// </summary>
    public const string SOURCE_NAME = "chaos-server";

    /// <summary>
    ///     The name of the update loop activity source.
    /// </summary>
    public const string UPDATE_SOURCE_NAME = "chaos-server.updates";

    /// <summary>
    ///     The name of the packet processing activity source.
    /// </summary>
    public const string PACKET_SOURCE_NAME = "chaos-server.packets";

    /// <summary>
    ///     The name of the world script execution activity source.
    /// </summary>
    public const string WORLD_SCRIPT_SOURCE_NAME = "chaos-server.worldscripts";

    /// <summary>
    ///     The general ActivitySource for miscellaneous traces.
    /// </summary>
    public static readonly ActivitySource Source = new(SOURCE_NAME);

    /// <summary>
    ///     The ActivitySource for update loop traces (map updates, entity updates, etc.).
    /// </summary>
    public static readonly ActivitySource Updates = new(UPDATE_SOURCE_NAME);

    /// <summary>
    ///     The ActivitySource for packet processing traces.
    /// </summary>
    public static readonly ActivitySource Packets = new(PACKET_SOURCE_NAME);

    /// <summary>
    ///     The ActivitySource for world script execution traces.
    /// </summary>
    public static readonly ActivitySource WorldScripts = new(WORLD_SCRIPT_SOURCE_NAME);

    /// <summary>
    ///     Starts a new general activity with the given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartActivity(string name, ActivityKind kind = ActivityKind.Internal) => Source.StartActivity(name, kind);

    /// <summary>
    ///     Starts a new packet processing activity with the given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartPacketActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => Packets.StartActivity(name, kind);

    /// <summary>
    ///     Starts a new update loop activity with no parent (root activity), sampled independently.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartRootUpdateActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var previous = Activity.Current;
        Activity.Current = null;

        var activity = Updates.StartActivity(name, kind);

        // Restore previous context for other code paths (though typically not needed in async)
        if (activity == null)
            Activity.Current = previous;

        return activity;
    }

    /// <summary>
    ///     Starts a new world script execution activity with no parent (root activity), sampled independently.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartRootWorldScriptActivity(string name, ActivityKind kind = ActivityKind.Internal)
    {
        var previous = Activity.Current;
        Activity.Current = null;

        var activity = WorldScripts.StartActivity(name, kind);

        if (activity == null)
            Activity.Current = previous;

        return activity;
    }

    /// <summary>
    ///     Starts a new update loop activity with the given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartUpdateActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => Updates.StartActivity(name, kind);

    /// <summary>
    ///     Starts a new world script execution activity with the given name.
    /// </summary>
    /// <param name="name">
    ///     The name of the activity/span.
    /// </param>
    /// <param name="kind">
    ///     The kind of activity (default is Internal).
    /// </param>
    /// <returns>
    ///     The started Activity, or null if no listener is registered.
    /// </returns>
    public static Activity? StartWorldScriptActivity(string name, ActivityKind kind = ActivityKind.Internal)
        => WorldScripts.StartActivity(name, kind);
}