#region
using Chaos.Networking.Abstractions;
using Chaos.NLog.Logging.Definitions;
using Chaos.NLog.Logging.Extensions;
using Microsoft.Extensions.Logging;
using TDigestNet;
#endregion

namespace Chaos.Networking;

/// <summary>
///     Represents statistical metrics for network packet execution times grouped by operation code.
/// </summary>
public sealed class NetworkStatistic
{
    /// <summary>
    ///     The average execution time in milliseconds for packets with this operation code.
    /// </summary>
    public double Average { get; init; }

    /// <summary>
    ///     The total number of packets recorded for this operation code.
    /// </summary>
    public long Count { get; init; }

    /// <summary>
    ///     The maximum execution time in milliseconds recorded for packets with this operation code.
    /// </summary>
    public double Max { get; init; }

    /// <summary>
    ///     The median execution time in milliseconds for packets with this operation code.
    /// </summary>
    public double Median { get; init; }

    /// <summary>
    ///     The network operation code identifying the type of packet being measured.
    /// </summary>
    public byte OpCode { get; init; }

    /// <summary>
    ///     The 95th percentile execution time in milliseconds for packets with this operation code.
    /// </summary>
    public double Upper95ThPercentile { get; init; }
}

/// <summary>
///     Stores the execution time of network packets. This is used to monitor the performance of the network layer.
/// </summary>
public sealed class NetworkMonitor
{
    private readonly SocketClientBase Client;
    private readonly ConcurrentDictionary<byte, TDigest> Digests;
    private readonly ILogger Logger;
    private Task MonitorLoopTask;

    /// <summary>
    ///     Initializes a new instance of the <see cref="NetworkMonitor" /> class
    /// </summary>
    public NetworkMonitor(SocketClientBase client, ILogger logger)
    {
        Client = client;
        Logger = logger;
        Digests = new ConcurrentDictionary<byte, TDigest>();

        MonitorLoopTask = RunMonitorLoop();
    }

    /// <summary>
    ///     Digests the execution time of a network packet.
    /// </summary>
    public void Digest(byte opcode, TimeSpan executionTime)
        => Digests.AddOrUpdate(
            opcode,
            _ =>
            {
                var newDigest = new TDigest();
                newDigest.Add(executionTime.Ticks);

                return newDigest;
            },
            (_, digest) =>
            {
                digest.Add(executionTime.Ticks);

                return digest;
            });

    private void PrintStatistics()
    {
        var digests = Digests.ToArray();

        if (digests.Length == 0)
            return;

        var serverTopic = Client switch
        {
            ILobbyClient => Topics.Servers.LobbyServer,
            ILoginClient => Topics.Servers.LoginServer,
            _            => Topics.Servers.WorldServer
        };

        var logEvent = Logger.WithTopics(serverTopic, Topics.Entities.Client, Topics.Entities.NetworkMonitor)
                             .WithProperty(Client);

        var statistics = new List<NetworkStatistic>(digests.Length);

        foreach ((var opcode, var digest) in digests.OrderBy(d => d.Key))
        {
            var average = digest.Average / TimeSpan.TicksPerMillisecond;
            var max = digest.Max / TimeSpan.TicksPerMillisecond;
            var count = (long)digest.Count;
            var upperPct = digest.Quantile(0.95) / TimeSpan.TicksPerMillisecond;
            var median = digest.Quantile(0.5) / TimeSpan.TicksPerMillisecond;

            var statistic = new NetworkStatistic
            {
                OpCode = opcode,
                Average = average,
                Max = max,
                Upper95ThPercentile = upperPct,
                Median = median,
                Count = count
            };

            statistics.Add(statistic);
        }

        Digests.Clear();

        logEvent.WithProperty(statistics)
                .LogTrace("Network Monitor [{@ClientId}]", Client.Id);
    }

    private Task RunMonitorLoop()
        => Task.Run(async () =>
        {
            var periodicTimer = new PeriodicTimer(TimeSpan.FromMinutes(1));

            while (Client.Connected)
                try
                {
                    await periodicTimer.WaitForNextTickAsync()
                                       .ConfigureAwait(false);

                    PrintStatistics();
                } catch
                {
                    //ignored
                }
        });
}