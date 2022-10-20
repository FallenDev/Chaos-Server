using Chaos.Containers;
using Chaos.Schemas.Aisling;
using Chaos.TypeMapper.Abstractions;

namespace Chaos.MapperProfiles;

public sealed class UserOptionsSchemaMapperProfile : IMapperProfile<UserOptions, UserOptionsSchema>
{
    public UserOptions Map(UserOptionsSchema obj) => new()
    {
        Exchange = obj.Exchange,
        FastMove = obj.FastMove,
        Group = obj.FastMove,
        GuildChat = obj.GuildChat,
        Magic = obj.GuildChat,
        Shout = obj.Shout,
        Whisper = obj.Whisper,
        Wisdom = obj.Wisdom
    };

    public UserOptionsSchema Map(UserOptions obj) => new()
    {
        Exchange = obj.Exchange,
        FastMove = obj.FastMove,
        Group = obj.Group,
        GuildChat = obj.GuildChat,
        Magic = obj.Magic,
        Shout = obj.Shout,
        Whisper = obj.Whisper,
        Wisdom = obj.Wisdom
    };
}