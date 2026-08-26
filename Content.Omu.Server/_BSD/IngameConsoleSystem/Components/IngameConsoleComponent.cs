using Content.Omu.Shared._BSD.IngameConsoleSystem;

namespace Content.Omu.Server._BSD.IngameConsoleSystem.Components;

[RegisterComponent]
public sealed partial class IngameConsoleComponent : Component
{
    /// <summary>
    /// allowed 
    /// </summary>
    [DataField]
    public HashSet<IngameConsoleCommandType> AllowedTypes = new();

    /// <summary>
    /// allowed 
    /// </summary>
    [DataField]
    public List<string> History = ["Start"];
}