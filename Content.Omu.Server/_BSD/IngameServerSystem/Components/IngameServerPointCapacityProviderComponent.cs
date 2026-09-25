using Content.Omu.Server._BSD.IngameServerSystem.Prototype;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.IngameServerSystem.Components;

[RegisterComponent]

public sealed partial class IngameServerPointCapacityComponent : Component
{
    [DataField]
    public Dictionary<ProtoId<IngameServerPointPrototypePrototype>, int> CapacityExpansion = new();
}