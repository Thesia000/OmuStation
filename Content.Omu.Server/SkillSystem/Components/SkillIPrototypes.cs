using Robust.Shared.Prototypes;

namespace Content.Omu.Server.SkillSystem.Components;

[Prototype]
public sealed partial class SkillNamePrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = string.Empty;
}

[Prototype]
public sealed partial class DepartmentNamePrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = string.Empty;
}

[Prototype]
public sealed partial class SkillLevelPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = string.Empty;
}