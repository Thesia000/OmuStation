using System.Numerics;
using System.Runtime.InteropServices;
using Content.Goobstation.Shared._BSD.Drive.Components;


namespace Content.Goobstation.Shared._BSD.Drive;

public abstract class SharedBluespaceStationDriveCoreSystem : EntitySystem
{
    [Dependency] protected readonly IGameTiming Timing = default!;
    public override void Initialize()
    {
        base.Initialize();
    }
    #region EnergyManagment
    //called by the associated drive every update tick, so thats where we getting the drives component
    public void EnergyDecay(BluespaceStationDriveCoreComponent component)
    {
        var containerSys = _entityManager.System<SharedContainerSystem>();
        if (component == null)
        {
            return;
        }
        BluespaceStationDriveComponent drive;
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
    public void BeamEnergy(BluespaceStationDriveCoreComponent component)
    {
        if (component == null)
        {
            return;
        }
        component.Energy += component.DeltaEnergyBeams;
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
        var distance = component.Distance;
        deltaChange += distance - component.InnerShellDistance//distance to the inner ring
                        + component.outerShellDistance - distance//distance to the outer ring
                        - Math.Log10(component.Energy);//mallus for high energy
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
            if (deltaChange > component.HardStability)
            {
                deltaChange -= component.HardStability;
                component.HardStability = 0;//TRIGGER core failure
            }
            else
            {
                component.HardStability -= deltaChange;
            }
        }
        if (component.HardStability < 0 && component.CoreSaftyOverwriteActive)
        {
            if (deltaChange > component.CoreStability)
            {
                deltaChange -= component.CoreStability;
                component.CoreStability = 0;//--> NUKE the station or do the special thing
                var ev = new CoreFailureEvent();
                RaiseLocalEvent(uid, ev, true);
            }
            else
            {
                component.CoreStability -= deltaChange;
            }
        }
    }
    //add some detection for consequences, that are not nuking
    public void EvaluateStability(BluespaceStationDriveCoreComponent component)
    {

    }
    #endregion
    #region Movment
    public void CoreVirtualMove(BluespaceStationDriveCoreComponent component)
    {
        if (component == null)
        {
            return;
        }
        //Energy beams
        Complex angleTrans = (0, component.Angle);
        float currentX = Complex.Exp(angleTrans).Real * component.Distance;
        float currentY = Complex.Exp(angleTrans).Imaginary * component.Distance;
        float deltaX = 0f;
        float deltaY = 0f;
        if (component.ActiveEnergyBeams[0])//N
        {
            deltaY += Math.Exp(currentY) * component.ActiveEnergyBeamsPower[0];
        }
        if (component.ActiveEnergyBeams[1])//N
        {
            deltaY -= Math.Exp(-currentY) * component.ActiveEnergyBeamsPower[1];
        }
        if (component.ActiveEnergyBeams[2])//N
        {
            deltaY += Math.Exp(currentX) * component.ActiveEnergyBeamsPower[2];
        }
        if (component.ActiveEnergyBeams[3])//N
        {
            deltaY -= Math.Exp(currentX) * component.ActiveEnergyBeamsPower[3];
        }
        currentX += deltaX;
        currentY += deltaY;
        component.Angle = Math.Atan2(currentX, currentY);
        component.Distance = Math.Sqrt(Math.Pow(currentX, 2) + Math.Pow(currentY, 2));
        //Distance
        float deltaChange = 0f;
        deltaChange += component.Distance - component.InnerShellDistance;//for now linear scaling, consider exponental later
        deltaChange += component.OuterShellDistance - component.Distance;
        component.Distance += deltaChange;
        //Rotation
        float deltaAngle = component.Distance / component.RotationSpeed;
        component.Angle += deltaAngle;
        while (component.Angle >= Math.PI * 2)
        {
            component.Angle -= Math.PI * 2;
            var ev = new BasePointRewardEvent();
            RaiseLocalEvent(uid, ev, true);
        }
    }
    #endregion
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var coreList = EntityQueryEnumerator<BluespaceStationDriveCoreComponent>();
        while (coreList.MoveNext(out var ent, out var core))
        {
            EnergyDecay(core);
            StabilityUpdate(core);
            EvaluateStability(core);
            CoreVirtualMove(core);
        }
    }
}