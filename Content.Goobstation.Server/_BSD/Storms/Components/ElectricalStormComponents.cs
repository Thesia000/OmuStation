namespace Content.Goobstation.Server._BSD.Storms.Components;

[RegisterComponent]
public sealed partial class ElectricalStormComponent : Component
{
    /// <summary>
    /// The storms strenth
    /// </summary>
    [DataField("stormIntensity")]
    public float StormIntensity = 1f;

    /// <summary>
    /// links the prototype that the shadow storm spawns
    /// </summary>
    [DataField("spawnPrototype")]
    public string SpawnPrototype = "ElectricalStormIndicator";

}

[RegisterComponent]
public sealed partial class ElectricalStormIndicatorComponent : Component
{
    /// <summary>
    /// The linked storms strength, needed for damage calc
    /// </summary>
    [DataField("stormIntensity")]
    public float StormIntensity = 1f;

    /// <summary>
    /// The time at which the next Storm pulse will occur. 
    /// This time is calculated based on the Intensity of the storm
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan NextPhaseTime = TimeSpan.Zero;

    /// <summary>
    /// The time between phase changes
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public TimeSpan DefaultPhaseTime = TimeSpan.FromSeconds(1);

    /// <summary>
    /// ticks up at phase 6 we trigger the damage event 
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public int Phase = 0;

}