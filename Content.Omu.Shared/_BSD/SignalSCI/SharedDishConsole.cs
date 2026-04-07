using Robust.Shared.Serialization;

namespace Content.Omu.Shared._BSD.SignalSCI.SharedDishConsole;

[Serializable, NetSerializable]
public enum DishConsoleUiKey : byte
{
    Key
}

[Serializable, NetSerializable]
public sealed class DishConsoleBoundUserInterfaceState : BoundUserInterfaceState
{
    public float RequestedAngle;

    public DishConsoleBoundUserInterfaceState(float requestedAngle)
    {
        RequestedAngle = requestedAngle;
    }
}

[Serializable, NetSerializable]
public sealed class DishConsolePrintDiskMessage : BoundUserInterfaceMessage
{
    
}

[Serializable, NetSerializable]
public sealed class DishConsoleSetRequestedAngleMessage : BoundUserInterfaceMessage
{
    public float RequestedAngle;

    public DishConsoleSetRequestedAngleMessage(float requestedAngle)
    {
        RequestedAngle = requestedAngle;
    }
}