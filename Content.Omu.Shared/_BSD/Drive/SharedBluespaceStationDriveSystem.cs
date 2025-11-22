using Content.Goobstation.Shared._BSD.Drive.Components;
using Content.Goobstation.Shared._BSD.Storms.Components;
using Content.Goobstation.Shared._BSD.Storms.Events;

namespace Content.Goobstation.Shared._BSD.Drive;
/*
public abstract class SharedBluespaceStationDriveSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }
    #region The navmap
    /*
    The navmap is not a history but only the term for the options offered to the stations crew when performing a jump.

    This part of the navmap is there to generate , save, manipulate and delete the options the station has. 
    *//*
    private void UpdateDistance(BluespaceStationDriveComponent component)
    {
        for (int iterator = 0; iterator < component.NavMapNodes.Length; iterator++)
        {
            if (component.NavMapNodes[iterator].NodeID != component.DestinationMapNavNodeId)
            {
                continue;
            }
            component.NavMapNodes[iterator].Distance -= component.DriveVelocity;
            if (component.NavMapNodes[iterator].Distance <= 0)
            {
                ArrivalAtNode(component);//needs to be added
            }
            return;
        }
    }
    private void ArrivalAtNode(BluespaceStationDriveComponent component)//mainly generate a new NavMap and possibly other effects.
    {
        int totalNodes = component.NavMapUpwardsChoises + component.NavMapHorizontalChoises + component.NavMapDownwardsChoises;
        component.CurrentMapNode = component.NavMapNodes[component.DestinationMapNavNodeId];
        /*
        Create the new navmap
        *//*
        int nodeNR = 0;
        NavMapNode[] creration = new NavMapNode[totalNodes];
        for (int i; i <= component.NavMapDownwardsChoises; i++)
        {
            NavMapNode node = new NavMapNode();
            node.Depth = Math.Max(0, component.Depth - 1);
            node.Distance = 0;
            node.BluespaceResistance = 0;
            //node.StormIntensities;//array -> kind of shit needs a better way, possibly a key -> value system?
            node.NodeID = nodeNR;
            creration[nodeNR] = node;
            nodeNR++;
        }
        for (int i; i <= component.NavMapHorizontalChoises; i++)
        {
            NavMapNode node = new NavMapNode();
            node.Depth = component.Depth;
            node.Distance = 0;
            node.BluespaceResistance = 0;
            //node.StormIntensities;//array -> kind of shit needs a better way, possibly a key -> value system?
            node.NodeID = nodeNR;
            creration[nodeNR] = node;
            nodeNR++;
        }
        for (int i; i <= component.NavMapDownwardsChoises; i++)
        {
            NavMapNode node = new NavMapNode();
            node.Depth = component.Depth;
            node.Distance = 0;
            node.BluespaceResistance = 0;
            //node.StormIntensities;//array -> kind of shit needs a better way, possibly a key -> value system?
            node.NodeID = nodeNR;
            creration[nodeNR] = node;
            nodeNR++;
        }
        //creration.copy(component.NavMapNodes);
        /*
        Set the storms
        */
        //works on a key -> value system, raises a event for each storm type
        /*
        storm types:
        "shadow"
        "electric"
        "fire"
        *//*
        var ev = new StormDataEvent(component.currentMapNode.StormIntensities);
        RaiseLocalEvent(uid, ev, true);
        return;
    }
    public void UpdateAcceleration(BluespaceStationDriveComponent component, float deltaChange)//move to server
    {
        component.Acceleration += deltaChange;
        return;
    }
    #endregion
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var driveList = EntityQueryEnumerator<BluespaceStationDriveComponent>();
        while (driveList.MoveNext(out var ent, out var drive))
        {
            UpdateDistance(drive);
        }
    }

}*/