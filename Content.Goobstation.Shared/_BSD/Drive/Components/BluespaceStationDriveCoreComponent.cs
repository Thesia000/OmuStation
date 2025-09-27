

namespace Content.Goobstation.Shared._BSD.Drive.Components;

[RegisterComponent]
public sealed partial class BluespaceStationDriveCoreComponent : Component
{
    /// <summary>
    /// Current Energy
    /// </summary>
    [DataField("energy")]
    public float Energy = 0f;

    /// <summary>
    /// Current soft stability, can be regenerated
    /// </summary>
    [DataField("softStability")]
    public float SoftStability = 100f;

    /// <summary>
    /// Current hard stability, needs specia repairs
    /// </summary>
    [DataField("hardStability")]
    public float HardStability = 100f;

    /// <summary>
    /// Current core stability, only happens during nukies, cant be repaired
    /// </summary>
    [DataField("coreStability")]
    public float CoreStability = 100f;
    /// <summary>
    /// Allowes nuclear operatives to overload the core and kill everbody
    /// </summary>
    [DataField("coreSaftyOverwriteActive")]
    public bool CoreSaftyOverwriteActive = true;

    /// <summary>
    /// the position X in the virtual grid
    /// </summary>
    [DataField("positionX")]
    public float PositionX = 0f;
    /// <summary>
    /// the position Y in the virtual grid
    /// </summary>
    [DataField("positionY")]
    public float PositionY = 0f;
    /// <summary>
    /// The efficency of the energy used for traveling(between 1 and 0)
    /// </summary>
    [DataField("travelEfficency")]
    public float TravelEfficency = 1f;
}

