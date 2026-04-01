
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

namespace Content.Omu.Server._BSD.SignalSCI;

/// <summary>
/// This system handles the signal dish multiblock behaviour
/// </summary>
public sealed partial class SignalDishSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _MapSys = default!;
    [Dependency] protected readonly SharedTransformSystem _trans = default!;
    [Dependency] protected readonly SignalMapSystem _signalMap = default!;
    [Dependency] private readonly ResearchSystem _research = default!;
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSciDishComponent, MultiStructChangeEvent>(UpdateValues);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var query = EntityQueryEnumerator<SignalSciDishComponent>();
        while (query.MoveNext(out var dishEnt, out var comp))
        {
            if (comp.Harvesting)//basicly if this machine is turned on -> needs to be move to multi struct possibly
            {
                DishSignalHarvest(dishEnt, comp);
            }
            RotationUpdate(dishEnt, comp);
        }
    }
    private void RotationUpdate(EntityUid uid, SignalSciDishComponent comp)
    {
        TryComp<TransformComponent>(uid, out var transcomp);
        float angle=(float)transcomp.WorldRotation;
        if(100.0f*MathF.Abs(MathF.Tan((angle - comp.DesiredAngle) / 180.0f))<comp.AngleErrorMargine)return;//IMPORTANT the diviation is exponental dont mess with angle ERROR margine less you KNOW what you are doing
        float maxRotation = -1 * MathF.Tan((angle - comp.DesiredAngle) / 180.0f);//contant to be added probably going to be modular
        TryComp<TransformComponent>(transcomp.GridUid,out var gridTransformComp);
        Angle newAngel = (Angle)((float)gridTransformComp.WorldRotation + maxRotation);
        if(newAngel > 360.0f)newAngel-=360.0f;
        if(newAngel < 0.0f)newAngel+=360.0f;
        _trans.SetWorldRotation(gridTransformComp,newAngel);
        return;
    }
    private void UpdateValues(EntityUid uid, SignalSciDishComponent comp, ref MultiStructChangeEvent args)
    {
        if(!TryComp<MultiBlockStructureComponent>(uid, out var structureComp))return;
        //harvesting rate
        comp.HarvestingRate = 0f;
        foreach(string ProviderType in comp.DishTypes)
        {
            if(!structureComp.TypesPresent.ContainsKey(ProviderType))continue;
            comp.HarvestingRate += structureComp.TypesPresent[ProviderType] * comp.HarvestingBaseRate;
        }
        //Conversion efficency
        comp.EfficencyConversion = comp.EfficencyBase;
        foreach(string ProviderType in comp.ProductivityTypes)
        {
            if(!structureComp.TypesPresent.ContainsKey(ProviderType))continue;
            comp.EfficencyConversion += structureComp.TypesPresent[ProviderType];//productivity modules always have efficency rating coresponding to there buff!!!
        }
        return;
    }

    private void DishSignalHarvest(EntityUid uid, SignalSciDishComponent dishComp)
    {
        EntityUid MapUid = _MapSys.GetMapOrInvalid(Transform(uid).MapID);
        if(!TryComp<SignalMapComponent>(MapUid, out var comp))
        {
            comp = _signalMap.SetupMapSignals(MapUid);
        }
        TryComp<TransformComponent>(uid, out var transcomp);
        float angle=(float)transcomp.WorldRotation;
        if(comp == null){Log.Error("MapEnt: "+MapUid+" did not contain the SignalMapComponent but was expected to.");return;}
        for(int move = 0; move < comp.SignalList.Count; move++)//this math needs to be done every tick as we can harvest multiple signals if they align
        {
            float efficency = 1.0f;
            if(angle - comp.SignalList[move].Angle != 0f){
                //the magic numbers used here are used to achive a repaeating tan function that has a periodicity of 360.0f currently fine tuned for a 6 degree missaligment before penelties
                efficency = MathF.Min(MathF.Abs(MathF.Tan((angle - comp.SignalList[move].Angle + 180.0f) / (4.0f * 180.0f / (2 * (float)MathF.PI))) / 10.0f), 1.0f);
            }
            if(efficency > 0f)
            {
                float harvestedAmount = Math.Min(efficency * dishComp.HarvestingRate, comp.SignalList[move].DataRemaining);
                comp.SignalList[move].DataRemaining -= harvestedAmount;
                if(dishComp.LinkedServer==null)continue;
                if(!TryComp<ResearchServerComponent>(dishComp.LinkedServer, out var serverComp))continue;
                //serverComp.StoredData += harvestedAmount * dishComp.EfficencyConversion;future math
                _research.ModifyServerPoints(dishComp.LinkedServer, (int)Math.Round(harvestedAmount * dishComp.EfficencyConversion));//temporarly direct conversion time
            }
        }
        return;
    }
}