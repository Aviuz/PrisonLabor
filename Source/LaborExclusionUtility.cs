using Verse;

namespace PrisonLabor
{
    internal class LaborExclusionUtility
    {
        public static bool IsDisabledByLabor(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
        {
            if (pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null &&
                !PrisonLaborUtility.WorkDisabled(workType))
                return pawn.Map.areaManager.Get<Area_Labor>()[pos];
            return false;
        }
    }
}