namespace Content.Omu.Shared.IngameConsoleSystem.Components;

[RegisterComponent]
public sealed partial class IngameConsoleComponent : Component
{
    /// <summary>
    /// Components Present entity dic of connected to stated server
    /// </summary>
    [DataField]
    public HashSet<IngameConsoleCommandType> AllowedTypes = new();
}