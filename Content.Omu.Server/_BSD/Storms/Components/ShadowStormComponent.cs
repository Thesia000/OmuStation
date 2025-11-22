namespace Content.Goobstation.Server._BSD.Storms.Components;

[RegisterComponent]
public sealed partial class ShadowStormComponent : Component
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
    public string SpawnPrototype = "StormShadow";

}