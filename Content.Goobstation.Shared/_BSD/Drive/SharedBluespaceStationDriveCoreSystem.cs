using Content.Goobstation.Shared._BSD.Drive.Components;


namespace Content.Goobstation.Shared._BSD.Drive;

public abstract class SharedBluespaceStationDriveCoreSystem : EntitySystem
{
    public override void Initialize()
    {
        base.Initialize();
    }
    #region EnergyManagment
    //called by the associated drive every update tick, so thats where we getting the drives component
    public void EnergyDecay(BluespaceStationDriveCoreComponent component, BluespaceStationDriveComponent drive)
    {
        if (component == null)
        {
            return;
        }
        var deltaChange = 0f;
        var stability = component.SoftStability + component.HardStability - 100f;
        if (drive.Traveling)
        {
            deltaChange -= drive.Depth * component.TravelEfficency * (1f / stability + 2f);//magic number to prevent div by 0
        }
        deltaChange -= component.Energy * (1f / stability + 2f);//magic number to prevent div by 0
        component.Energy += deltaChange;
        //call event for acceleration change proportional to energy decay after all
    }
    public void BeamEnergy(BluespaceStationDriveCoreComponent component, float BeamEnergy)
    {
        if (component == null)
        {
            return;
        }
        component.Energy += BeamEnergy;
        return;
    }
    #endregion
    #region Stability
    public void StabilityUpdate(BluespaceStationDriveCoreComponent component)
    {
        if (component == null)
        {
            return;
        }
        var deltaChange = 0f;
        var distance = Math.Sqrt(component.PositionY * component.PositionY + component.PositionX * component.PositionX);
        deltaChange += -(distance * distance - Math.Log(distance)) + 2f - Math.Log(component.Energy);
        if (component.SoftStability > 0)
        {
            if (deltaChange > component.SoftStability)
            {
                deltaChange -= component.SoftStability;
                component.SoftStability = 0;
            }
            else
            {
                component.SoftStability -= deltaChange;
            }
        }
        if (component.SoftStability < 0)
        {
            if (deltaChange > component.SoftStability)
            {
                deltaChange -= component.HardStability;
                component.HardStability = 0;
            }
            else
            {
                component.HardStability -= deltaChange;
            }
        }
    }
    #endregion
}