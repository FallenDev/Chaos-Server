using Chaos.Common.Identity;
using Chaos.Common.Utilities;

namespace Chaos.Services.Other;

public sealed class BoardKeyMapper : KeyMapper<ushort>
{
    /// <inheritdoc />
    public BoardKeyMapper()
        : base(new SequentialIdGenerator<ushort>()) { }

    public string? GetKey(ushort id)
    {
        foreach ((var key, var value) in Map)
            if (value == id)
                return key;

        return null;
    }
}