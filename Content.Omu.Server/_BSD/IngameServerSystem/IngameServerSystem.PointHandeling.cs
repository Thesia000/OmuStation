using Content.Omu.Server._BSD.IngameServerSystem.Helpers;
using Content.Omu.Server._BSD.IngameServerSystem.Components;
using Content.Omu.Server._BSD.IngameServerSystem.Events;
using System.Linq;

namespace Content.Omu.Server._BSD.IngameServerSystem
{
    public sealed partial class BSDIngameServerSystem : EntitySystem
    {
        /// <summary>
        /// returns (int) 0 in case there is no IngameServerComponent on the given entity
        /// </summary>
        /// <param name="uid">Entity UID of the Server in question</param>
        /// <param name="type">Type of point you are trying to add</param>
        /// <returns></returns>
        public int GetMaxPointAddition(EntityUid uid, string type)
        {
            if (!TryComp<IngameServerComponent>(uid, out var serverComp))
                return 0;
            return GetMaxPointAddition(serverComp, type);
        }

        public int GetMaxPointAddition(IngameServerComponent serverComp, string type)
        {
            if (!serverComp.StoredPointsCapacity.ContainsKey(type) || serverComp.StoredPointsCapacity[type] == null)
                return int.MaxValue;
            if (!serverComp.StoredPoints.ContainsKey(type))
                serverComp.StoredPoints.Add(type, 0);//ensure we have the type present we want to check for better safe than sorry
            return (int) serverComp.StoredPointsCapacity[type]! - serverComp.StoredPoints[type];
        }
        /// <summary>
        /// Tries to add the maximum amount of points possible returns the amount of points it was unable to add
        /// </summary>
        /// <param name="serverComp"></param>
        /// <param name="type"></param>
        /// <param name="deltaAdd"></param>
        /// <returns></returns>
        public int TryAddMaxPoints(IngameServerComponent serverComp, string type, int deltaAdd)
        {
            int toAdd = Math.Min(deltaAdd, GetMaxPointAddition(serverComp, type));
            if (!serverComp.StoredPoints.ContainsKey(type))
            {
                serverComp.StoredPoints.Add(type, 0);
            }
            serverComp.StoredPoints[type] += toAdd;
            return deltaAdd - toAdd;
        }
        public int TryAddMaxPoints(EntityUid entityUid, string type, int deltaAdd)
        {
            if (!TryComp<IngameServerComponent>(entityUid, out var comp))
                return deltaAdd;
            return TryAddMaxPoints(comp, type, deltaAdd);
        }
    }
}