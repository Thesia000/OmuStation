
using Robust.Shared.Collections;
using Robust.Shared.Map.Components;
using Robust.Shared.GameObjects;

using Content.Server.Research.Systems;
using Content.Shared.Research.Components;
using Content.Shared.Research;

using Content.Omu.Server._BSD.SignalSCI.Components;
using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;
using Content.Omu.Server._BSD.MultiBlockSystem;

using Content.Omu.Shared._BSD.IngameConsoleSystem;


namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSys = default!;
    [Dependency] private readonly SharedTransformSystem _trans = default!;
    [Dependency] private readonly SignalMapSystem _signalMap = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent, MultiStructChangeEvent>(UpdateValuesMultiStruct);
        SubscribeLocalEvent<SignalSciDishComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommand);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciDishComponent, MultiBlockEnergyManagmentComponent>();
        while (query.MoveNext(out var dishEnt, out var comp, out var energycomp))
        {
            //if (!energycomp.Powered) continue;//do nothing if no power is in the system -> we ignore power for testing
            if (comp.Harvesting)//basicly if this machine is turned on -> needs to be move to multi struct possibly
            {
                DishSignalHarvest(dishEnt, comp);
            }
            RotationUpdate(dishEnt, comp);
        }
    }
    public void IngameConsoleCommand(Entity<SignalSciDishComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        if (args.Type == IngameConsoleCommandType.ICC_SET && args.Args!.Length > 2)
        {
            IngameConsoleHistoryChangeEvent ev = new(Loc.GetString("SSI_Dish_Orientation_Set", ("Variable", args.Args[1]), ("Value", args.Args[2])));
            RaiseLocalEvent(ent, ref ev);
            UpdateVariableIngameConsoleCommand(args.Args[1], args.Args[2], ent.Comp);
        }
    }
    private void UpdateVariableIngameConsoleCommand(string varID, string value, SignalSciDishComponent comp)
    {
        int intParsRes = 0;//switch does not like local declaration so we doing it this way
        switch (varID)
        {
            case ("rotation_1"):
                if (!int.TryParse(value, out intParsRes)) return;
                comp.DesiredAngles[0] = intParsRes % 360;
                return;
            case ("rotation_2"):
                if (!int.TryParse(value, out intParsRes)) return;
                comp.DesiredAngles[1] = intParsRes % 360;
                return;
            case ("rotation_3"):
                if (!int.TryParse(value, out intParsRes)) return;
                comp.DesiredAngles[2] = intParsRes % 360;
                return;
            default:
                break;
        }
    }
    private void RotationUpdate(EntityUid uid, SignalSciDishComponent comp)
    {
        RotationUpdateUnobservedDimention(uid, comp);
        RotationUpdateObservedDimention(uid, comp);//we only need to do visual changes for one dimention as the game is in 2d and not 3d OR even 4d
        return;
    }
    private void RotationUpdateObservedDimention(EntityUid uid, SignalSciDishComponent comp)
    {
        if (!TryComp<MultiBlockStructureComponent>(uid, out var multistructcomp)) return;//consider making this a proper methode to call
        if (!multistructcomp.EntityDic.ContainsKey("SignalAntenna")) return;
        EntityUid antennaUid = multistructcomp.EntityDic["SignalAntenna"][0].Id;//gets the first entry
        TransformComponent transcomp = Transform(antennaUid);
        if (transcomp.GridUid == null) return;
        TransformComponent gridTransformComp = Transform(transcomp.GridUid!.Value);
        Angle newAngel = (Angle) comp.CurrentAngles[0] + transcomp.LocalRotation;
        _trans.SetWorldRotation(gridTransformComp, newAngel);
        return;
    }
    private void RotationUpdateUnobservedDimention(EntityUid uid, SignalSciDishComponent comp)
    {
        for (int iterator = 0; iterator < 3; iterator++)
        {
            float angle = comp.CurrentAngles[iterator] * (180.0f / MathF.PI);
            float maxRotation = 0f;
            if (angle <= comp.AngleErrorMargine + comp.DesiredAngles[iterator] && angle >= comp.DesiredAngles[iterator] - comp.AngleErrorMargine)
            {
                continue;
            }
            maxRotation = comp.DesiredAngles[iterator] - angle;
            if (maxRotation > 360.0f + angle - comp.DesiredAngles[iterator]) maxRotation = 360.0f + angle - comp.DesiredAngles[iterator];
            maxRotation = MathF.Max(MathF.Min(maxRotation, comp.MaxRotationSpeed), -1 * comp.MaxRotationSpeed) * (float) (Math.PI / 180.0f);
            Angle newAngel = (Angle) (comp.CurrentAngles[iterator] + maxRotation);
            comp.CurrentAngles[iterator] = (float) newAngel;
        }
    }
    private void UpdateValuesMultiStruct(EntityUid uid, SignalSciDishComponent comp, ref MultiStructChangeEvent args)
    {
        if (!TryComp<MultiBlockStructureComponent>(uid, out var structureComp)) return;
        //harvesting rate
        comp.HarvestingRate = 0f;
        foreach (string providerType in comp.DishTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            comp.HarvestingRate += structureComp.TypesPresent[providerType] * comp.HarvestingBaseRate;
        }
        //Conversion efficency
        comp.EfficencyConversion = comp.EfficencyBase;
        foreach (string providerType in comp.ProductivityTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            comp.EfficencyConversion += structureComp.TypesPresent[providerType];//productivity modules always have efficency rating coresponding to there buff!!!
        }
        return;
    }

    private void DishSignalHarvest(EntityUid uid, SignalSciDishComponent dishComp)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(uid).MapID);
        if (!TryComp<SignalMapComponent>(mapUid, out var comp))
        {
            comp = _signalMap.SetupMapSignals(mapUid);
        }
        float angle = (float) _trans.GetWorldRotation(uid);
        if (comp == null)
        {
            Log.Error("MapEnt: " + mapUid + " did not contain the SignalMapComponent but was expected to.");
            return;
        }
        for (int move = 0; move < comp.SignalList.Count; move++)//this math needs to be done every tick as we can harvest multiple signals if they align
        {
            float efficency = 1.0f;
            if (angle - comp.SignalList[move].Angle != 0f)
            {
                //the magic numbers used here are used to achive a repaeating tan function that has a periodicity of 360.0f currently fine tuned for a 6 degree missaligment before penelties
                efficency = MathF.Min(MathF.Abs(MathF.Tan((angle - comp.SignalList[move].Angle + 180.0f) / (4.0f * 180.0f / (2 * (float) MathF.PI))) / 10.0f), 1.0f);
            }
            if (efficency > 0f)
            {
                float harvestedAmount = Math.Min(efficency * dishComp.HarvestingRate, comp.SignalList[move].DataRemaining);
                comp.SignalList[move].DataRemaining -= harvestedAmount;
                //if (!TryComp<SignalSciServerComponent>(dishComp.LinkedServer, out var serverComp)) continue;
                //serverComp.StoredData += harvestedAmount * dishComp.EfficencyConversion;
                //_research.ModifyServerPoints(dishComp.LinkedServer, (int)Math.Round(harvestedAmount * dishComp.EfficencyConversion));//temporarly direct conversion time
            }
        }
        return;
    }
}