namespace Content.Omu.Shared._BSD.SignalSCI.Components;

[RegisterComponent]

public sealed partial class SignalSciSignalComponent : Component
{
    /// <summery>
    /// Always a positive integer and above 0 or we devide by 0
    /// </summery>      
    [ViewVariables(VVAccess.ReadWrite)]
    public float RemainingData = 1000f;

    /// <summery>
    /// Always a positive integer and above 0 or we devide by 0
    /// </summery>      
    [ViewVariables(VVAccess.ReadWrite)]
    public bool HasEvent = false;
    /// <summery>
    /// Always a positive integer and above 0 or we devide by 0
    /// </summery>      
    [ViewVariables(VVAccess.ReadWrite)]
    public bool SpawnGrid = false;
    [ViewVariables(VVAccess.ReadWrite)]
    public string Grid;

    /// <summery>
    /// Always a positive integer and above 0 or we devide by 0
    /// </summery>      
    [ViewVariables(VVAccess.ReadWrite)]
    public bool AddGamerule = false;
    [ViewVariables(VVAccess.ReadWrite)]
    public string Gamerule;

    /// <summery>
    /// Always a positive integer and above 0 or we devide by 0
    /// </summery>      
    [ViewVariables(VVAccess.ReadWrite)]
    public bool Anouncment = false;
    [ViewVariables(VVAccess.ReadWrite)]
    public string AnouncmentText;
    [ViewVariables(VVAccess.ReadWrite)]
    public string AnouncmentOrigin;



}
