#region
using Chaos.Extensions.Common;
using OpenTelemetry.Trace;
#endregion

namespace Chaos.Definitions;

/// <summary>
///     A custom sampler that applies different sampling ratios based on the activity source.
/// </summary>
public sealed class ChaosTracingSampler : Sampler
{
    private readonly Sampler DefaultSampler;
    private readonly Sampler PacketSampler;
    private readonly Sampler UpdateSampler;
    private readonly Sampler WorldScriptSampler;

    /// <summary>
    ///     Initializes a new instance of the <see cref="ChaosTracingSampler" /> class.
    /// </summary>
    /// <param name="defaultRatio">
    ///     The sampling ratio for general traces (0.0 to 1.0).
    /// </param>
    /// <param name="updateRatio">
    ///     The sampling ratio for update loop traces (0.0 to 1.0).
    /// </param>
    /// <param name="packetRatio">
    ///     The sampling ratio for packet processing traces (0.0 to 1.0).
    /// </param>
    /// <param name="worldScriptRatio">
    ///     The sampling ratio for world script execution traces (0.0 to 1.0).
    /// </param>
    public ChaosTracingSampler(
        double defaultRatio,
        double updateRatio,
        double packetRatio,
        double worldScriptRatio)
    {
        DefaultSampler = CreateSampler(defaultRatio);
        UpdateSampler = CreateSampler(updateRatio);
        PacketSampler = CreateSampler(packetRatio);
        WorldScriptSampler = CreateSampler(worldScriptRatio);
    }

    private static Sampler CreateSampler(double ratio)
        => ratio switch
        {
            <= 0.0 => new AlwaysOffSampler(),
            >= 1.0 => new AlwaysOnSampler(),
            _      => new TraceIdRatioBasedSampler(ratio)
        };

    /// <inheritdoc />
    public override SamplingResult ShouldSample(in SamplingParameters samplingParameters)
    {
        var sourceName = samplingParameters.Tags?.FirstOrDefault(t => t.Key == "otel.library.name")
                                           .Value as string;

        // If we can't determine the source from tags, check the activity name prefix
        if (string.IsNullOrEmpty(sourceName))
        {
            var activityName = samplingParameters.Name;

            if (activityName.StartsWithI("Update") || activityName.StartsWithI("Map."))
                return UpdateSampler.ShouldSample(samplingParameters);

            if (activityName.StartsWithI("Packet") || activityName.StartsWithI("Recv") || activityName.StartsWithI("Send"))
                return PacketSampler.ShouldSample(samplingParameters);

            if (activityName.StartsWithI("WorldScript"))
                return WorldScriptSampler.ShouldSample(samplingParameters);

            return DefaultSampler.ShouldSample(samplingParameters);
        }

        return sourceName switch
        {
            ChaosActivitySource.UPDATE_SOURCE_NAME       => UpdateSampler.ShouldSample(samplingParameters),
            ChaosActivitySource.PACKET_SOURCE_NAME       => PacketSampler.ShouldSample(samplingParameters),
            ChaosActivitySource.WORLD_SCRIPT_SOURCE_NAME => WorldScriptSampler.ShouldSample(samplingParameters),
            _                                            => DefaultSampler.ShouldSample(samplingParameters)
        };
    }
}