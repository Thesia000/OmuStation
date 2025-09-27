

namespace Content.Goobstation.Shared._BSD.Drive.Components;

[RegisterComponent]
public sealed partial class BluespaceStationDriveComponent : Component
{
    /// <summary>
    /// The target of the next jump
    /// </summary>
    [DataField("destinationMapNavNodeId")]
    public int DestinationMapNavNodeId = 0;

    /// <summary>
    /// the depth in the nodemap the station is curretnly on
    /// </summary>
    [DataField("depth")]
    public int Depth = 0;

    /// <summary>
    /// If the drive is currently causing a bluespace travel
    /// </summary>
    [DataField("traveling")]
    public bool Traveling = false;

    /// <summary>
    /// Current acceleration of the drive
    /// </summary>
    [DataField("acceleration")]
    public float Acceleration = 0f;

    /// <summary>
    /// Current Velocity
    /// </summary>
    [DataField("driveVelocity")]
    public float DriveVelocity = 0f;

    /// <summary>
    /// Current Velocity
    /// </summary>
    [DataField("navMapNodes")]
    public NavMapNode[] NavMapNodes;

    /// <summary>
    /// The target of the next jump
    /// </summary>
    [DataField("navMapUpwardsChoises")]
    public int NavMapUpwardsChoises = 1;

    /// <summary>
    /// The target of the next jump
    /// </summary>
    [DataField("navMapHorizontalChoises")]
    public int NavMapHorizontalChoises = 0;

    /// <summary>
    /// The target of the next jump
    /// </summary>
    [DataField("navMapDownwardsChoises")]
    public int NavMapDownwardsChoises = 1;

}

public struct NavMapNode
{
    public int NodeID;
    public int Depth;
    public float Distance;
    public float BluespaceResistance;
    public int[] StormIntensities;
}
