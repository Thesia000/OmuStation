
using Content.Omu.Server._BSD.MultiBlockSystem.Components;
using Robust.Shared.Map;

namespace Content.Omu.Server._BSD.MultiBlockSystem;

public sealed partial class MultiBlockSystem : EntitySystem
{
    /// <summary>
    /// Function that ensure that the given origin point is unable to reach the edge of the structure,
    /// functions primary intend for machines that have hazadros effects if not completly contained and ill effects to happen in case of a breach
    /// TODO: make a future algorithm that is able to detect the proper breach location -> odd shapes lead to interesting results rn
    /// </summary>
    /// <param name="xOrigin"></param>
    /// <param name="yOrigin"></param>
    /// <param name="blockingLayers"></param>
    /// <param name="compStruct"></param>
    /// <param name="breaches"> outputs the locations of the EDGE of the structure MAP NOT the actual location of the breach of none square structures!!!</param>
    /// <returns></returns>
    public bool ValidateContainment(int xOrigin, int yOrigin, string[] blockingLayers, MultiBlockStructureComponent compStruct, out List<EntityCoordinates> breaches)
    {
        breaches = new();
        return false;
    }
}