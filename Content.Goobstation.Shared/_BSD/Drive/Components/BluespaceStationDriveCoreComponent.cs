

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
    /// The incomming energy into the system
    /// </summary>
    [DataField("deltaEnergyBeams")]
    public float DeltaEnergyBeams = 0f;

    /// <summary>
    /// List of active beams, index accending: 0,1,2,3 -> N,S,E,W
    /// </summary>
    [DataField("activeEnergyBeams")]
    public bool[] ActiveEnergyBeams = [false, false, false, false];

    /// <summary>
    /// List of active beam power, index accending: 0,1,2,3 -> N,S,E,W
    /// </summary>
    [DataField("activeEnergyBeamsPower")]
    public float[] DeltaEnergyBeams = [0f, 0f, 0f, 0f];

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
    /// the andgle the core point is at, in 2i*PI*n where n is this value
    /// </summary>
    [DataField("angle")]
    public float Angle = 0f;

    /// <summary>
    /// the distance from (0,0) in the virtual grid
    /// </summary>
    [DataField("distance")]
    public float Distance = 0f;

    /// <summary>
    /// the distance from (0,0) in the virtual grid
    /// </summary>
    [DataField("innerShellDistance")]
    public float InnerShellDistance = 0f;

    /// <summary>
    /// the distance from (0,0) in the virtual grid
    /// </summary>
    [DataField("outerShellDistance")]
    public float OuterShellDistance = 0f;

    /// <summary>
    /// the andgle the core point is at
    /// </summary>
    [DataField("moveDistance")]
    public float BaseMoveDistance = 10f;

    /// <summary>
    /// The efficency of the energy used for traveling(between 1 and 0)
    /// </summary>
    [DataField("travelEfficency")]
    public float TravelEfficency = 1f;

    /// <summary>
    /// ID of the related drive
    /// </summary>
    [DataField("driveID")]
    public EntityUid DriveId = 0;
}

