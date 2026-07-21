using Content.Shared.Materials;
using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.SignalSalv.Components;

[RegisterComponent]
public sealed partial class SignalSalvPlanetResourcesComponent : Component
{
    /// <summary>
    /// Mining rate
    /// </summary>
    [DataField, AutoNetworkedField]
    public Dictionary<ProtoId<MaterialPrototype>, int> MiningRates { get; set; } = new();

    /// <summary>
    /// if the planet contains advanced resources -> gold, silver, uranium
    /// </summary>
    [DataField]
    public bool AdvancedResourcePlanet = false;

    /// <summary>
    /// if the planet contains special resources -> plasma
    /// </summary>
    [DataField]
    public bool SpecialResourcePlanet = false;
}
