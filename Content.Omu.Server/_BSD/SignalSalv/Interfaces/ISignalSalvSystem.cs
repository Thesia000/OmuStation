
using JetBrains.Annotations;
using Content.Omu.Server._BSD.SignalSalv.Components;

namespace Content.Omu.Server._BSD.SignalSalv
{
    [PublicAPI]
    public partial interface IBSDSignalSalvSystem
    {
        bool ValidMaterial(String materialType);
        bool ValidateMap(int number, out EntityUid? mapUid);
        SignalSalvMaterialTransitMapComponent SetupMapMaterialTransitComp(EntityUid mapUid);
        void OverrideProductionRate(SignalSalvMaterialTransitMapComponent comp, string type, int newAmount);
    }
}