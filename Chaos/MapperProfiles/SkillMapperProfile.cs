using Chaos.Networking.Entities.Server;
using Chaos.Objects.Panel;
using Chaos.Schemas.Aisling;
using Chaos.Scripting.Abstractions;
using Chaos.Storage.Abstractions;
using Chaos.Templates;
using Chaos.TypeMapper.Abstractions;
using Microsoft.Extensions.Logging;

namespace Chaos.MapperProfiles;

public sealed class SkillMapperProfile : IMapperProfile<Skill, SkillSchema>,
                                         IMapperProfile<Skill, SkillInfo>
{
    private readonly ILogger<SkillMapperProfile> Logger;
    private readonly IScriptProvider ScriptProvider;
    private readonly ISimpleCache SimpleCache;

    public SkillMapperProfile(
        ISimpleCache simpleCache,
        IScriptProvider scriptProvider,
        ILogger<SkillMapperProfile> logger
    )
    {
        SimpleCache = simpleCache;
        ScriptProvider = scriptProvider;
        Logger = logger;
    }

    public Skill Map(SkillSchema obj)
    {
        var template = SimpleCache.Get<SkillTemplate>(obj.TemplateKey);

        var skill = new Skill(
            template,
            ScriptProvider,
            obj.ScriptKeys,
            obj.UniqueId,
            obj.ElapsedMs)
        {
            Slot = obj.Slot ?? 0
        };

        Logger.LogTrace("Deserialized skill - Name: {SkillName}, UniqueId: {UniqueId}", skill.Template.Name, skill.UniqueId);

        return skill;
    }

    public Skill Map(SkillInfo obj) => throw new NotImplementedException();

    SkillInfo IMapperProfile<Skill, SkillInfo>.Map(Skill obj) => new()
    {
        Name = obj.Template.Name,
        Slot = obj.Slot,
        Sprite = obj.Template.PanelSprite
    };

    public SkillSchema Map(Skill obj)
    {
        var ret = new SkillSchema
        {
            UniqueId = obj.UniqueId,
            ElapsedMs = obj.Elapsed.HasValue ? Convert.ToInt32(obj.Elapsed.Value.TotalMilliseconds) : null,
            ScriptKeys = obj.ScriptKeys.Except(obj.Template.ScriptKeys).ToHashSet(StringComparer.OrdinalIgnoreCase),
            TemplateKey = obj.Template.TemplateKey,
            Slot = obj.Slot
        };

        Logger.LogTrace("Serialized skill - TemplateKey: {TemplateKey}, UniqueId: {UniqueId}", ret.TemplateKey, ret.UniqueId);

        return ret;
    }
}