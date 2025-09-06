using Content.Shared.Explosion;
using Robust.Shared.Serialization.TypeSerializers.Implementations.Custom.Prototype;

namespace Content.Goobstation.Server._BSD.Storms.Components;

[RegisterComponent]
public sealed partial class FireStormComponent : Component
{
    /// <summary>
    /// The explosion prototype to spawn
    /// </summary>
    [DataField("pulseDetonation", required: true, customTypeSerializer: typeof(PrototypeIdSerializer<ExplosionPrototype>))]
    public string ExplosionPrototype;

    /// <summary>
    /// The total amount of intensity an explosion can achieve
    /// </summary>
    [DataField("explosionTotalIntensity")]
    public float ExplosionTotalIntensity = 40f;

    /// <summary>
    /// How quickly does the explosion's power slope? Higher = smaller area and more concentrated damage, lower = larger area and more spread out damage
    /// </summary>
    [DataField("explosionDropoff")]
    public float ExplosionDropoff = 5f;

    /// <summary>
    /// How much intensity can be applied per tile?
    /// </summary>
    [DataField("explosionMaxTileIntensity")]
    public float ExplosionMaxTileIntensity = 1f;

    /// <summary>
    /// How much intensity can be applied per tile?
    /// </summary>
    [DataField("stormIntensity")]
    public float StormIntensity = 1f;
}