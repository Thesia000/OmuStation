using Content.Goobstation.Shared._BSD.Drive.Components;


namespace Content.Goobstation.Shared._BSD.Drive;

public abstract class SharedStormSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }
    #region The navmap
    /*
    The navmap is not a history but only the term for the options offered to the stations crew when performing a jump.

    This part of the navmap is there to generate , save, manipulate and delete the options the station has. 
    */
    private void UpdateDistance(BluespaceStationDriveComponent component, float deltaTime)
    {
        for (int iterator = 0; iterator < component.NavMapNodes.Length; iterator++)
        {
            if (component.NavMapNodes[iterator].NodeID != component.DestinationMapNavNodeId)
            {
                continue;
            }
            component.NavMapNodes[iterator].Distance -= component.DriveVelocity * deltaTime;
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
        return;
    }


    #endregion

}