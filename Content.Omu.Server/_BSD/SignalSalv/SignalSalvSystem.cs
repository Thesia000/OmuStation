
using Content.Omu.Shared.IngameConsoleSystem;

using Content.Omu.Server._BSD.SignalSalv.Components;
using Content.Omu.Server._BSD.SignalSalv.Events;
using Content.Omu.Server._BSD.SignalSalv.Helpers;

using Content.Omu.Server._BSD.MultiBlockSystem.Events;
using Content.Omu.Server._BSD.MultiBlockSystem.Components;

using Robust.Shared.Timing;

using System.Linq;
using System.Numerics;
using Robust.Shared.Utility;

using Robust.Shared.Prototypes;

using Content.Shared.Materials;
using Content.Shared.Interaction;
using Robust.Shared.Random;
using Robust.Shared.EntitySerialization.Systems;
using Robust.Shared.Map;
using Robust.Shared.Toolshed.TypeParsers.Math;
using Content.Goobstation.Shared.Wraith.SaltLines;
using System.Diagnostics.Metrics;

using Content.Server.Parallax;
using Content.Shared.Parallax.Biomes;
using Robust.Shared.Physics.Components;
using Robust.Shared.Toolshed.Commands.Values;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Content.Shared.Ninja.Systems;

namespace Content.Omu.Server._BSD.SignalSalv;

public sealed partial class SignalSalvSystem : EntitySystem
{
    [Dependency] private readonly SharedMapSystem _mapSys = default!;
    [Dependency] private readonly IGameTiming _timing = default!;
    [Dependency] private readonly SharedMaterialStorageSystem _material = default!;
    [Dependency] private readonly MapLoaderSystem _mapLoader = default!;
    [Dependency] private readonly BiomeSystem _biome = default!;
    [Dependency] private readonly IPrototypeManager _protoManager = default!;

    private static readonly TotalMaterialMiningRateList MaterialMiningRatesBase = new();
    private static readonly ProtoId<BiomeTemplatePrototype> BiomeTemplate = "Continental";
    public override void Initialize()
    {
        base.Initialize();
        SubscribeLocalEvent<SignalSalvMaterialTransitMapComponent, SignalSalvMiningRigProductionChangeEvent>(UpdateProductionRates);
        SubscribeLocalEvent<SignalSalvMaterialReciverStructureComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommandMatReciver);
        SubscribeLocalEvent<SignalSalvFtlDeviceComponent, IngameConsoleCommandCalledEvent>(IngameConsoleCommandSignalSalvFTLDevice);
        SubscribeLocalEvent<SignalSalvMiningRigStructreComponent, MultiStructChangeEvent>(MiningRigRecalculationStructureChange);
        SubscribeLocalEvent<SignalSalvOutpostDataComponent, AfterInteractEvent>(OnAfterInteractOutpostData);
    }
    public override void Update(float frameTime)
    {
        base.Update(frameTime);
        var queryMatInTransit = EntityQueryEnumerator<SignalSalvMaterialTransitMapComponent>();
        while (queryMatInTransit.MoveNext(out var mapEnt, out var comp))
        {
            MaterialProductionTimeBased(mapEnt, comp);
        }
    }
    #region User Interfacing
    public void IngameConsoleCommandMatReciver(Entity<SignalSalvMaterialReciverStructureComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {

        // assign reciver | sets this machine to be the material reciver
        if (args.Type == IngameConsoleCommandType.ICC_ASSIGN && args.Args!.Length > 1 && args.Args[1] == "reciver")
        {
            ChangeMaterialReciverOnTransitComp(ent);
            IngameConsoleHistoryChangeEvent ev = new("Material destination changed");
            RaiseLocalEvent(ent, ref ev);
            //Now add stuff to history to update that it worked;
        }
        else if (args.Type == IngameConsoleCommandType.ICC_Print && args.Args!.Length > 1 && args.Args[1] == "materials")
        {
            IngameConsoleHistoryChangeEvent ev = new(PrintMaterialInbound(ent));
            RaiseLocalEvent(ent, ref ev);
        }
    }
    public void IngameConsoleCommandSignalSalvFTLDevice(Entity<SignalSalvFtlDeviceComponent> ent, ref IngameConsoleCommandCalledEvent args)
    {
        if (args.Type == IngameConsoleCommandType.SSA_FTL)
        {
            IngameConsoleHistoryChangeEvent ev = new("-> FTL Attempt started");
            RaiseLocalEvent(ent, ref ev);
            GenerateExpeditionMapAndFTL(ent, ent.Comp);
        }
        else if (args.Type == IngameConsoleCommandType.ICC_Print && args.Args!.Length > 1 && args.Args[1] == "ftl")
        {
            IngameConsoleHistoryChangeEvent ev = new(PrintFTLStatus(ent));
            RaiseLocalEvent(ent, ref ev);
        }
    }
    private string PrintMaterialInbound(EntityUid uidReciver)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(uidReciver).MapID);
        if (!TryComp<SignalSalvMaterialReciverStructureComponent>(uidReciver, out var compReciver)) return "ERROR- THIS IS NOT A MATERIAL RECIVER";
        if (!TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp))
        {
            comp = SetupMapMaterialTransitComp(mapUid);
        }
        string returnString = "";
        if (comp.MaterialInTransit.Keys == null) return returnString;
        foreach (var iterator in comp.MaterialInTransit.Keys)//TEMP REPLACE WITH PROPER LOCALISATION LATER!!!!
        {
            string subAddition = "";
            subAddition += (string) iterator;
            subAddition += " -> stored off site:";
            subAddition += comp.MaterialInTransit[iterator].ToString();
            subAddition += " | next delivery at Offsite storage of: ";
            subAddition += compReciver.MaterialCargoMin.ToString();
            if (comp.MaterialProductionPerSecond.ContainsKey(iterator))
            {
                subAddition += " | Offsite mining rate: ";
                subAddition += comp.MaterialProductionPerSecond[iterator].ToString();
            }
            else
            {
                subAddition += " | No offsite mining detected";
            }
            subAddition += "\n(";
            int counter = 1;
            while (counter < 11)
            {
                if (comp.MaterialInTransit[iterator] > (comp.MaterialProductionPerSecond[iterator] / (counter / 10.0f)))
                {
                    subAddition += "|";
                }
                else
                {
                    subAddition += "-";
                }
                counter++;
            }
            subAddition += ")\n";
            returnString += subAddition;
        }
        /*
        The above code creats something akin to this:
        Material -> stored([Amount in transit])|delivery at([Amount needed to arrive])|production([Productionrate]/s)
        Material (Progress bar)
        example:
        Stell -> stored(10)|delivery at(20)|production(0.1/s)
        Steel [|||||-----]
        Glass -> stored(15)|delivery at(20)|production(0.15/s)
        Glass [|||||||---]
        */
        return returnString;
    }
    private string PrintFTLStatus(EntityUid uidReciver)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(uidReciver).MapID);
        if (!TryComp<SignalSalvFtlDeviceComponent>(uidReciver, out var compFTL)) return "ERROR- THIS IS NOT A FTL DRIVE";
        TransformComponent transComp = Transform(uidReciver);
        Vector2d vector = new(transComp.Coordinates.X, transComp.Coordinates.Y);
        string returnString = "";
        returnString += "-> FTL Capacitor Charge: " + compFTL.StoredChargeFTLCapacitiors + " out of required: " + compFTL.FTLCharge;
        returnString += "\n(";
        int counter = 1;
        while (counter < 11)
        {
            if (compFTL.StoredChargeFTLCapacitiors > (compFTL.FTLCharge / (counter / 10.0f)))
            {
                returnString += "|";
            }
            else
            {
                returnString += "-";
            }
            counter++;
        }
        returnString += ")\n";
        if (compFTL.JumpPointSet)
        {
            returnString += "-> Jump Point at location: (" + compFTL.DesignatedJumpPoint.X + "|" + compFTL.DesignatedJumpPoint.Y + ")\n";
            returnString += "-> Approximate Jump Point distance: " + GetDistance(vector, compFTL.DesignatedJumpPoint);
        }
        else
        {
            returnString += "-> Jump point is yet to be calculated";
        }
        return returnString;
    }
    #endregion
    #region Material Transit
    public void UpdateProductionRates(Entity<SignalSalvMaterialTransitMapComponent> ent, ref SignalSalvMiningRigProductionChangeEvent args)
    {
        foreach (var iterator in args.NewProductionRate.Keys)
        {
            ent.Comp.MaterialProductionPerSecond[iterator] -= args.OldProductionRate[iterator];
            ent.Comp.MaterialProductionPerSecond[iterator] += args.NewProductionRate[iterator];
        }
        return;
    }
    public SignalSalvMaterialTransitMapComponent SetupMapMaterialTransitComp(EntityUid mapUid)
    {
        EnsureComp<SignalSalvMaterialTransitMapComponent>(mapUid);//ensure the map of the station has signals
        TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp);
        return comp!;
    }
    public void ChangeMaterialReciverOnTransitComp(EntityUid newDestination)
    {
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(newDestination).MapID);
        if (!TryComp<SignalSalvMaterialTransitMapComponent>(mapUid, out var comp))
        {
            comp = SetupMapMaterialTransitComp(mapUid);
        }
        comp.LinkedMaterialReciver = newDestination;
        return;
    }
    public void MaterialProductionTimeBased(EntityUid mapUid, SignalSalvMaterialTransitMapComponent comp)
    {
        TimeSpan deltaTime = _timing.CurTime - comp.LastUpdate;
        comp.LastUpdate = _timing.CurTime;
        int deltaTimeInt = (int) deltaTime.TotalMilliseconds;
        foreach (var iterator in comp.MaterialProductionPerSecond.Keys)
        {
            if (!comp.MaterialInTransit.ContainsKey(iterator))
            {
                comp.MaterialInTransit.Add(iterator, 0);
            }
            comp.MaterialInTransit[iterator] += comp.MaterialProductionPerSecond[iterator] * (int) (deltaTimeInt / 1000.0f);
            if (comp.MaterialInTransit[iterator] > comp.MaterialInTransitStorageCap) comp.MaterialInTransit[iterator] = comp.MaterialInTransitStorageCap;
        }
        //exit in case we dont have a linked reciver
        if (!TryComp<SignalSalvMaterialReciverStructureComponent>(comp.LinkedMaterialReciver, out var reciverComp)) return;
        if (!TryComp<MaterialStorageComponent>(comp.LinkedMaterialReciver, out var matStorageComp)) return;
        foreach (var iterator in comp.MaterialInTransit.Keys)
        {
            if (comp.MaterialInTransit[iterator] < reciverComp.MaterialCargoMin) continue;
            int deltaMaterialChange = Math.Min(reciverComp.MaterialCargoMin, _material.GetMaxAddableVolume(comp.LinkedMaterialReciver, matStorageComp, iterator));
            if (_material.TryChangeMaterialAmount(comp.LinkedMaterialReciver, iterator, deltaMaterialChange, matStorageComp)) continue;
            comp.MaterialInTransit[iterator] -= deltaMaterialChange;
        }
        return;
    }
    #endregion
    #region Mining Rig
    public void MiningRigRecalculationStructureChange(Entity<SignalSalvMiningRigStructreComponent> ent, ref MultiStructChangeEvent args)
    {
        if (!TryComp<MultiBlockStructureComponent>(ent, out var structureComp)) return;
        foreach (string providerType in ent.Comp.ProductivityTypes)
        {
            if (!structureComp.TypesPresent.ContainsKey(providerType)) continue;
            ent.Comp.ProductivityPoints += (int) structureComp.TypesPresent[providerType];
        }
        MiningRigRecalculation(ent, ent.Comp.MiningRateModifier);
        return;
    }
    public void OnAfterInteractOutpostData(Entity<SignalSalvOutpostDataComponent> ent, ref AfterInteractEvent args)
    {
        Random rand = new((int) _timing.CurTime.TotalSeconds);
        if (args.Handled || !args.CanReach || args.Target is not { } target)
            return;

        if (!HasComp<SignalSalvMiningRigStructreComponent>(target))
            return;
        float deltaData = ent.Comp.OutpostData;
        if (ent.Comp.OutpostDataRandom)
        {
            deltaData = rand.NextFloat(ent.Comp.OutpostDataMin, ent.Comp.OutpostDataMax);
        }
        TryAddAdditonalRandomMaterialToPlanetMining(target);
        MiningRigRecalculationOutpostDataChange(target, deltaData);
    }
    public bool TryAddAdditonalRandomMaterialToPlanetMining(EntityUid ent)
    {
        Random rand = new((int) _timing.CurTime.TotalSeconds);
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(ent).MapID);
        if (!TryComp<SignalSalvPlanetResourcesComponent>(mapUid, out var mapResourceComp)) return false;
        bool returnBool = false;
        int attempts;
        attempts = 10;
        while (attempts > 0)
        {
            if (mapResourceComp.AdvancedResourcePlanet)
            {
                //time to add a random special resource AND a advanced resource after -> advanced planets are good
                Material randomSpecialMat = MaterialMiningRatesBase.SpecialMaterials.ElementAt((int) rand.NextInt64(0, MaterialMiningRatesBase.SpecialMaterials.Count));
                if (!mapResourceComp.MiningRates.ContainsKey(randomSpecialMat.MaterialType))
                {
                    mapResourceComp.MiningRates.Add(randomSpecialMat.MaterialType, (int) rand.NextInt64(randomSpecialMat.MinResoucePerSecond, randomSpecialMat.MaxResoucePerSecond));
                    returnBool = true;
                    break;
                }
            }
            attempts--;
        }
        attempts = 10;
        while (attempts > 0)
        {
            Material randomAdvancedMat = MaterialMiningRatesBase.AdvancedMaterials.ElementAt((int) rand.NextInt64(0, MaterialMiningRatesBase.AdvancedMaterials.Count));
            if (!mapResourceComp.MiningRates.ContainsKey(randomAdvancedMat.MaterialType))
            {
                mapResourceComp.MiningRates.Add(randomAdvancedMat.MaterialType, (int) rand.NextInt64(randomAdvancedMat.MinResoucePerSecond, randomAdvancedMat.MaxResoucePerSecond));
                returnBool = true;
                break;
            }
            attempts--;
        }
        return returnBool;
    }
    public void MiningRigRecalculationOutpostDataChange(EntityUid ent, float deltaData)
    {
        if (!TryComp<SignalSalvMiningRigStructreComponent>(ent, out var comp)) return;
        comp.OutpostData += deltaData;
        Entity<SignalSalvMiningRigStructreComponent> passdownEnt = ent!;//we know this is not null
        MiningRigRecalculation(passdownEnt, comp.MiningRateModifier);
    }
    public void MiningRigRecalculation(Entity<SignalSalvMiningRigStructreComponent> ent, float oldMiningRateModifier)
    {
        ent.Comp.MiningRateModifier = 1;
        ent.Comp.MiningRateModifier += ent.Comp.GroundSurveyData;
        ent.Comp.MiningRateModifier += ent.Comp.OutpostData;
        ent.Comp.MiningRateModifier += (float) Math.Log(ent.Comp.ProductivityPoints, ent.Comp.ProductivityScalingBase);
        if (ent.Comp.MiningRateModifier == oldMiningRateModifier) return;
        Dictionary<ProtoId<MaterialPrototype>, int> oldMiningRates = new();
        Dictionary<ProtoId<MaterialPrototype>, int> newMiningRates = new();
        EntityUid mapUid = _mapSys.GetMapOrInvalid(Transform(ent).MapID);
        if (TryComp<SignalSalvPlanetResourcesComponent>(mapUid, out var mapResourceComp))
            foreach (var iterator in mapResourceComp.MiningRates.Keys)
            {
                oldMiningRates.Add(iterator, (int) (mapResourceComp.MiningRates[iterator] * oldMiningRateModifier));
                newMiningRates.Add(iterator, (int) (mapResourceComp.MiningRates[iterator] * ent.Comp.MiningRateModifier));
            }
        SignalSalvMiningRigProductionChangeEvent ev = new(oldMiningRates, newMiningRates);
    }
    #endregion
    #region Generate Map and travel
    public void GenerateExpeditionMapAndFTL(EntityUid shuttleConsole, SignalSalvFtlDeviceComponent ftlComp)
    {
        TransformComponent transComp = Transform(shuttleConsole);
        if (transComp.Anchored == false)
        {
            IngameConsoleHistoryChangeEvent ev = new("-> FTL CHANCELLED | FTL control is not anchored");
            RaiseLocalEvent(shuttleConsole, ref ev);
            return;
        }
        if (transComp.GridUid == null)
        {
            IngameConsoleHistoryChangeEvent ev = new("-> FTL CHANCELLED | Lack of a grid");
            RaiseLocalEvent(shuttleConsole, ref ev);
            return;
        }
        EntityUid gridUid = (EntityUid) transComp.GridUid;//we know you arent null
        //Check if the ship has FTL capabilities!!
        if (!CheckFTLAbility(gridUid, ftlComp))
        {
            IngameConsoleHistoryChangeEvent ev = new("-> FTL CHANCELLED | The grid is not FTL capable (check <print FTL> for details)");
            RaiseLocalEvent(shuttleConsole, ref ev);
            return;
        }
        //Generate the Map
        if (ftlComp.PreConfigedPlanet)
        {
            GenerateExpeditionMap();//for now the same fix later lamo
        }
        else
        {
            GenerateExpeditionMap();
        }
        //Move the ship

        return;
    }
    private bool CheckFTLAbility(EntityUid gridUid, SignalSalvFtlDeviceComponent ftlComp)
    {
        if (TryComp<PhysicsComponent>(gridUid, out var physicsComp)) return false;
        TransformComponent transComp = Transform(gridUid);
        if (physicsComp!.Mass > ftlComp.MaxFTLGridMass) return false;
        if (ftlComp.FTLCharge > ftlComp.StoredChargeFTLCapacitiors) return false;
        Vector2d vectorCord = new(transComp.Coordinates.X, transComp.Coordinates.Y);
        if (ftlComp.JumpPointTolerance > GetDistance(vectorCord, ftlComp.DesignatedJumpPoint)) return false;
        return true;
    }
    private float GetDistance(Vector2d a, Vector2d b)
    {
        var c = Math.Pow(a.X + b.X, 2);
        var d = Math.Pow(a.Y + b.Y, 2);
        return (float) Math.Sqrt(c + d);
    }
    public void GenerateJumpPoint(SignalSalvFtlDeviceComponent ftlComp)
    {
        Random rand = new((int) _timing.CurTime.TotalSeconds);
        float angle = rand.NextFloat(0, (float) Math.PI * 2f);
        ftlComp.DesignatedJumpPoint = (
            Math.Cos(angle) * ftlComp.DistanceFromZeroZeroForJumpPoint,
            Math.Sin(angle) * ftlComp.DistanceFromZeroZeroForJumpPoint
        );
        return;
    }
    public void GenerateExpeditionMap()
    {
        EntityUid expedMapUid = _mapSys.CreateMap(out var mapId);
        SignalSalvPlanetResourcesComponent planetResourcesComp = EnsureComp<SignalSalvPlanetResourcesComponent>(expedMapUid);
        GenerateExpeditionMap(expedMapUid, mapId, planetResourcesComp);
    }
    public void GenerateExpeditionMap(EntityUid expedMapUid, MapId mapId, SignalSalvPlanetResourcesComponent planetResourcesComp)
    {
        Random rand = new((int) _timing.CurTime.TotalSeconds);
        TotalMaterialMiningRateList matMiningList = new();
        //now generate stuff:
        foreach (var iterator in matMiningList.BaseMaterials)//all base materials are always present
        {
            planetResourcesComp.MiningRates.Add(iterator.MaterialType, (int) rand.NextInt64(iterator.MinResoucePerSecond, iterator.MaxResoucePerSecond));
        }
        if (planetResourcesComp.AdvancedResourcePlanet)//generate a randomly selected one
        {
            int randomIndex = (int) rand.NextInt64(matMiningList.AdvancedMaterials.Count);
            planetResourcesComp.MiningRates.Add(matMiningList.AdvancedMaterials.ElementAt(randomIndex).MaterialType,
                                            (int) rand.NextInt64(matMiningList.AdvancedMaterials.ElementAt(randomIndex).MinResoucePerSecond,
                                                                    matMiningList.AdvancedMaterials.ElementAt(randomIndex).MaxResoucePerSecond));
        }
        if (planetResourcesComp.SpecialResourcePlanet)//generate a randomly selected one
        {
            int randomIndex = (int) rand.NextInt64(matMiningList.SpecialMaterials.Count);
            planetResourcesComp.MiningRates.Add(matMiningList.SpecialMaterials.ElementAt(randomIndex).MaterialType,
                                            (int) rand.NextInt64(matMiningList.SpecialMaterials.ElementAt(randomIndex).MinResoucePerSecond,
                                                                    matMiningList.SpecialMaterials.ElementAt(randomIndex).MaxResoucePerSecond));
        }
        //time to add the POIs
        byte counter = rand.NextByte(planetResourcesComp.POIAmountMin, planetResourcesComp.POIAmountMax);
        HashSet<double> takenAngles = new();
        while (counter > 0)
        {
            counter--;
            ResPath pOIlocation = new();
            Vector2 offset = new();
            float distance = rand.NextFloat(planetResourcesComp.POIDistanceMin, planetResourcesComp.POIDistanceMax);
            int attempts = 0;
            bool invalidAngle = true;
            double angle = 0;
            while (invalidAngle || attempts < 15)
            {
                invalidAngle = false;
                angle = rand.NextFloat(0.0f, (float) Math.PI * 2.0f);
                foreach (var iterator in takenAngles)
                {
                    if (Math.Abs(angle - iterator) > planetResourcesComp.POIMinAngleDifference) invalidAngle = true;
                }
            }
            if (invalidAngle) continue;
            takenAngles.Add(angle);
            offset.X = (float) Math.Sin(angle) * distance;
            offset.Y = (float) Math.Cos(angle) * distance;
            _mapLoader.TryLoadGrid(mapId, pOIlocation, out var gridOut, null, offset);

            //Turn map into a planet and add the outer barrier
            _biome.EnsurePlanet(expedMapUid, _protoManager.Index(BiomeTemplate), rand.Next());
        }

    }
    #endregion
}