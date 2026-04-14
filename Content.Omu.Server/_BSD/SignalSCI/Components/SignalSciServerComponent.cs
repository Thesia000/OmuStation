namespace Content.Omu.Server._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciServerComponent : Component
{
    /// <summary>
    /// The name of the server
    /// </summary>
    [DataField]
    public string ServerName = "SIGNALSCI";
    /// <summary>
    /// The ID of the server
    /// </summary>
    [DataField]
    public int Id = 0;

    /// <summary>
    /// Data stored within this server
    /// </summary>
    [DataField]
    public float StoredData = 0f;

    /// <summary>
    /// Speed at witch the data is processed
    /// </summary>
    [DataField]
    public float ProcessingPower = 0f;

    /// <summary>
    /// The conversion rate for data to significant data
    /// </summary>
    [DataField]
    public float Efficency = 0.1f;

    /// <summary>
    /// Significant data stored within this server, used to print point diks, with special machines FTL disks, explorations and node maps
    /// </summary>
    [DataField]
    public float SignificantData = 0f;
}