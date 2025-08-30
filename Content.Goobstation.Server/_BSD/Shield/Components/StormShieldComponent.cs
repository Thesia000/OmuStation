
namespace Content.Goobstation.Server._BSD.Shield.Components;

[RegisterComponent]

public sealed partial class StormShieldComponent : Component
{
    /// <summary>
    /// Radius of the safe zone from storms
    /// </summary>
    [DataField("shieldRadius")]
    public float ShieldRadius = 5f;

    /// <summary>
    /// The amount of minimal energy needed
    /// </summary>
    [DataField("powerDrainBase")]
    public float PowerDrainBase = 100f;

    /// <summary>
    /// scaling power demand
    /// </summary>
    [DataField("powerDrainScaling")]
    public float PowerDrainScaling = 50f;
}