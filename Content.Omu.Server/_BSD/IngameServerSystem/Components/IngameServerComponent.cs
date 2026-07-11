using Robust.Shared.Prototypes;

using Content.Omu.Server._BSD.IngameServerSystem.Helpers;

using Content.Omu.Server._BSD.MultiBlockSystem.Components;

namespace Content.Omu.Server._BSD.IngameServerSystem.Components;

[RegisterComponent]

public sealed partial class IngameServerComponent : Component//there can only be ONE of these PER server, not sure if I want to attach the UI to this one
{
    [DataField]
    public Dictionary<IngameServerPoints, float> StoredPoints = new();

    [DataField]
    public float ProcessingPower = 0f;

    [DataField]
    public float AvailabeProcessingPower = 0f;

    [DataField]
    public HashSet<ProtoId<MultiStructTypePrototype>> ProcessingPowerProvidingTypes = new();

    [DataField]
    public Dictionary<IngameServerProgramTypes, IngameServerProgram> ActivePrograms = new();

    [DataField]
    public HashSet<float> InstalledPrograms = new();
}