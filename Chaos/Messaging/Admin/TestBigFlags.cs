#region
using Chaos.Collections.Common;
using Chaos.Definitions;
using Chaos.Messaging.Abstractions;
using Chaos.Models.World;
#endregion

namespace Chaos.Messaging.Admin;

[Command("testBigFlags")]
public class TestBigFlags : ICommand<Aisling>
{
    /// <inheritdoc />
    public ValueTask ExecuteAsync(Aisling source, ArgumentCollection args)
    {
        var value1 = BigFlagsTest.Test1.Value;
        var value2 = BigFlagsTest.Test2;
        var value3 = BigFlagsTest.Test3.Value;
        var value4 = BigFlagsTest.Test4;
        var value5 = BigFlagsTest.Test5.Value;

        value4 |= BigFlagsTest.Test3;

        source.SendActiveMessage($"{value1}, {value2}, {value3}, {value4}, {value5}");

        source.Trackers.BigFlags.AddFlag(value2 | value4);
        source.Trackers.BigFlags.AddFlag(BigFlagsTest.Test1);

        var stored = source.Trackers.BigFlags.GetFlag<BigFlagsTest>();

        source.SendActiveMessage($"{stored}");

        return default;
    }
}