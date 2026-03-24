namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciServerComponent : Component
{

    /// <summary>
    /// Data stored within this server
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float StoredData = 0f;

    /// <summary>
    /// Speed at witch the data is processed
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float ProcessingPower = 0f;

    /// <summary>
    /// The conversion rate for data to significant data
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float Efficency = 0.1f;

    /// <summary>
    /// Significant data stored within this server, used to print point diks, with special machines FTL disks, explorations and node maps
    /// </summary>
    [ViewVariables(VVAccess.ReadWrite)]
    public float SignificantData = 0f;


}