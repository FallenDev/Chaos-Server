#region
using Chaos.NLog.Logging.Extensions;
using FluentAssertions;
using Microsoft.Extensions.Logging;
#endregion

namespace Chaos.NLog.Logging.Tests;

public sealed class LoggerExtensionsTests
{
    [Test]
    public void WithMetrics_Adds_Metrics_Property_On_Log()
    {
        var baseLogger = new ListLogger();
        var logger = baseLogger.WithMetrics();

        logger.LogInformation("msg");

        baseLogger.Entries
                  .Should()
                  .HaveCount(1);
    }

    [Test]
    public void WithProperty_Adds_Topic_And_Property()
    {
        var baseLogger = new ListLogger();

        var logger = baseLogger.WithProperty(
            new
            {
                A = 1
            },
            "this");

        logger.LogInformation("msg");

        baseLogger.Entries
                  .Should()
                  .HaveCount(1);
    }

    private sealed class ListLogger : ILogger
    {
        public List<(LogLevel, string)> Entries { get; } = new();
        public IDisposable? BeginScope<TState>(TState state) where TState: notnull => null;
        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(
            LogLevel logLevel,
            EventId eventId,
            TState state,
            Exception? exception,
            Func<TState, Exception?, string> formatter)
            => Entries.Add((logLevel, formatter(state, exception)));
    }
}