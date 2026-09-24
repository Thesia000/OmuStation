
namespace Content.Omu.Server._BSD.SignalSalv;

public sealed partial class BSDSignalSalvSystem : EntitySystem, IBSDSignalSalvSystem
{
    public bool ValidMaterial(String materialType)
    {
        foreach (var iterator in MaterialMiningRatesBase.BaseMaterials)
        {
            if (iterator.MaterialType == materialType) return true;
        }
        foreach (var iterator in MaterialMiningRatesBase.AdvancedMaterials)
        {
            if (iterator.MaterialType == materialType) return true;
        }
        foreach (var iterator in MaterialMiningRatesBase.SpecialMaterials)
        {
            if (iterator.MaterialType == materialType) return true;
        }
        return false;
    }
    public bool ValidateMap(int number, out EntityUid? mapUid)
    {
        mapUid = null;
        if (!_mapSys.TryGetMap(new(number), out mapUid))
        {
            return false;
        }
        return true;
    }
}