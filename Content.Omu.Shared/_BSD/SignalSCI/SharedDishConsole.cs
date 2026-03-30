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
    public bool CanPrint;
    public int PointCost;
    public int ServerPoints;

    public DishConsoleBoundUserInterfaceState(int serverPoints, int pointCost, bool canPrint)
    {
        CanPrint = canPrint;
        PointCost = pointCost;
        ServerPoints = serverPoints;
    }
}

[Serializable, NetSerializable]
public sealed class DishConsolePrintDiskMessage : BoundUserInterfaceMessage
{
    
}