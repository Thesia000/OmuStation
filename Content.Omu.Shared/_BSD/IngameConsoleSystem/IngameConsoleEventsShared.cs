using Robust.Shared.Serialization;


namespace Content.Omu.Shared.IngameConsoleSystem;

[ByRefEvent]
public readonly record struct IngameConsoleCommandCalledEvent(IngameConsoleCommandType Type,
                                                                string[]? Args = null //still ships the type with it, aka start reading AFTER index 0
                                                                );//raised on the entity that is effecting it

[Serializable, NetSerializable]
public sealed class IngameConsoleCommandAttemptMessage : BoundUserInterfaceMessage
{
    public string InputString;

    public IngameConsoleCommandAttemptMessage(string requestedAngle)
    {
        InputString = requestedAngle;
    }
}