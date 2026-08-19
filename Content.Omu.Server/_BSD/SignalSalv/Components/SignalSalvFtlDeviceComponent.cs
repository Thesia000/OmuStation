using System.Numerics;
using Content.Shared.Containers.ItemSlots;
using Robust.Shared.Map;

namespace Content.Omu.Server._BSD.SignalSalv.Components;

//This exists to allow me to modify specail values later such as true distance of objects in LY and is also used as a console UI detection.

[RegisterComponent]
public sealed partial class SignalSalvFtlDeviceComponent : Component
{
    [DataField]
    public float MaxFTLGridMass = 100.0f;

    /// <summary>
    /// Amount of MJ needed to perform a FTL jump, yes this is a lot I recomend getting power from engi
    /// </summary>
    [DataField]
    public float FTLCharge = 10.0f;

    [DataField]
    public float FTLCapacitiorsStoredCharge = 0f;

    /// <summary>
    /// Amount of MJ moved to the FTL capacitors every second
    /// </summary>
    [DataField]
    public float FTLCapacitiorChargeRate = 1.0f;

    /// <summary>
    /// Amount of MJ moved to the FTL capacitors every second
    /// </summary>
    [DataField]
    public float FTLCapacitiorChargeEfficency = 0.25f;

    [DataField]
    public float DistanceFromZeroZeroForJumpPoint = 300.0f;

    [DataField]
    public float JumpPointTolerance = 25.0f;

    /// <summary>
    /// Location where the FTL jump will be taking place
    /// </summary>
    [DataField]
    public Vector2 DesignatedJumpPoint = new();

    /// <summary>
    /// Location where the FTL jump will be taking place
    /// </summary>
    [DataField]
    public bool JumpPointSet = false;

    /// <summary>
    /// Location where the FTL jump will be taking place
    /// </summary>
    [DataField]
    public bool PreConfigedPlanet = false;

    [DataField]
    public TimeSpan LastUpdate;

    /// <summary>
    /// save the map we are comming from (in case we somehow have multiple stations or station maps)
    /// </summary>
    [DataField]
    public EntityUid? Originmap;


    /// <summary>
    /// saved map we generated so we know what to delete when we FTL away and WHERE to FTL too
    /// </summary>
    [DataField]
    public EntityUid? CurrentlyLinkedMapUid;


    /// <summary>
    /// Stores the Datadisk for the planet generation
    /// </summary>
    [DataField]
    public ItemSlot GenerationSettingsDiskSlot = new();

    [DataField]
    public bool DeleteLinkedMapOnFTLArrival = false;
}
