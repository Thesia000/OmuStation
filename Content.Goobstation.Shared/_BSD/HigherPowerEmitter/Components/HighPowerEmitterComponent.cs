

namespace Content.Goobstation.Shared._BSD.Drive.Components;

[RegisterComponent]
public sealed partial class BluespaceStationDriveCoreComponent : Component
{
    /// <summary>
    /// Current EnergyConsumption
    /// </summary>
    [DataField("energy")]
    public float EnergyConsumption = 1000f;

    /// <summary>
    /// Energy efficency of the beam that is generated
    /// </summary>
    [DataField("energy")]
    public float EnergyEfficency = 0.5f;

    /// <summary>
    /// Inputable value via a console or direct access
    /// </summary>
    [DataField("energy")]
    public float DesiredBeamEnergy = 0f;

    /// <summary>
    /// Current Enery the beam actually transmits
    /// </summary>
    [DataField("energy")]
    public float CurrentBeamEnergy = 0f;

    /// <summary>
    /// Current FocuspointDistance
    /// </summary>
    [DataField("focusPoint")]
    public float FocusPointDistance = 10f;
}

