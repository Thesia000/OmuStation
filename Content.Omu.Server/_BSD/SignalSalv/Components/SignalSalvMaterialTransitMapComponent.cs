using Content.Shared.Materials;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.SignalSalv.Components;

[RegisterComponent]
/// This component is disconected and based on the MAP of the material silo to prevent a destruction of the silo to destroyed or other things happen to it
public sealed partial class SignalSalvMaterialTransitMapComponent : Component
{
    /// <summary>
    /// Amount of materials in transit
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<MaterialPrototype>, int> MaterialInTransit { get; set; } = new();

    /// <summary>
    /// Amount of materials in transit cap(100 ingots by default)
    /// </summary>
    [DataField]
    public int MaterialInTransitStorageCap = 100000;

    /// <summary>
    /// Amount of materials produced every second
    /// </summary>
    [DataField]
    public Dictionary<ProtoId<MaterialPrototype>, int> MaterialProductionPerSecond { get; set; } = new();

    /// <summary>
    /// Destination Material Reciver
    /// </summary>
    [DataField]
    public EntityUid LinkedMaterialReciver = new();

    [DataField]
    public TimeSpan LastUpdate = new();

}
