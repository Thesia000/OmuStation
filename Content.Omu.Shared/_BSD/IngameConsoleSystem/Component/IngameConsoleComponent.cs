namespace Content.Omu.Shared.IngameConsoleSystem.Components;

[RegisterComponent]
public sealed partial class IngameConsoleComponent : Component
{
    /// <summary>
    /// allowed 
    /// </summary>
    [DataField]
    public HashSet<IngameConsoleCommandType> AllowedTypes = new();
}