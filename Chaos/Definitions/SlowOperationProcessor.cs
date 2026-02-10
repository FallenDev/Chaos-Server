#region
using System.Diagnostics;
using Chaos.Networking.Abstractions.Definitions;
using OpenTelemetry;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     A span processor that ensures slow operations are always exported, regardless of sampling decisions. Fast
///     operations are subject to normal sampling, but any operation exceeding the threshold is guaranteed to be captured.
///     This processor should be added before the exporter in the pipeline.
/// </summary>
public sealed class SlowOperationProcessor : BaseProcessor<Activity>
{
    private readonly TimeSpan PacketThreshold;
    private readonly TimeSpan UpdateThreshold;
    private readonly TimeSpan WorldScriptThreshold;

    /// <summary>
    ///     Initializes a new instance of the <see cref="SlowOperationProcessor" /> class.
    /// </summary>
    /// <param name="updateThresholdMs">
    ///     Threshold in milliseconds for update operations.
    /// </param>
    /// <param name="packetThresholdMs">
    ///     Threshold in milliseconds for packet operations.
    /// </param>
    /// <param name="worldScriptThresholdMs">
    ///     Threshold in milliseconds for world script operations.
    /// </param>
    public SlowOperationProcessor(double updateThresholdMs, double packetThresholdMs, double worldScriptThresholdMs)
    {
        UpdateThreshold = TimeSpan.FromMilliseconds(updateThresholdMs);
        PacketThreshold = TimeSpan.FromMilliseconds(packetThresholdMs);
        WorldScriptThreshold = TimeSpan.FromMilliseconds(worldScriptThresholdMs);
    }

    /// <inheritdoc />
    public override void OnEnd(Activity data)
    {
        var isSlow = ShouldForceExport(data);

        // If this was sampled out but is slow, force it to be recorded
        if (isSlow && !data.Recorded)

            // Force the activity to be recorded by setting the flag
            data.ActivityTraceFlags |= ActivityTraceFlags.Recorded;

        // Mark slow operations with error status
        if (isSlow)
        {
            data.SetTag("slow_operation", true);
            data.SetTag("duration_ms", data.Duration.TotalMilliseconds);
            data.SetStatus(ActivityStatusCode.Error, "Operation exceeded duration threshold");
        } else if (data.Recorded && data.GetTagItem("duration_ms") is null)

            // Add duration tag for all recorded activities
            data.SetTag("duration_ms", data.Duration.TotalMilliseconds);
    }

    private bool ShouldForceExport(Activity data)
    {
        var sourceName = data.Source.Name;
        var duration = data.Duration;

        return sourceName switch
        {
            ChaosActivitySources.UPDATE_SOURCE_NAME       => duration > UpdateThreshold,
            NetworkingActivitySources.PACKET_SOURCE_NAME  => duration > PacketThreshold,
            ChaosActivitySources.WORLD_SCRIPT_SOURCE_NAME => duration > WorldScriptThreshold,
            _                                             => false // General traces use normal sampling only
        };
    }
}