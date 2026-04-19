using Robust.Shared.Prototypes;

namespace Content.Omu.Server._BSD.ServerClientLink.Prototypes;

/// <summary>
/// Prototype representing a struct Types in YAML.
/// Meant to only have an ID property, as that is the only thing that
/// gets saved in hashsets.
/// </summary>
[Prototype("ServerCleintProto")]
public sealed partial class ServerClientPrototype : IPrototype
{
    [IdDataField, ViewVariables]
    public string ID { get; private set; } = string.Empty;
}