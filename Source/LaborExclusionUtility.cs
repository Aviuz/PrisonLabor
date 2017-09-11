using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using RimWorld;

namespace PrisonLabor
{
    class LaborExclusionUtility
    {
        public static bool IsDisabledByLabor(IntVec3 pos, Pawn pawn, WorkTypeDef workType)
        {
            if (pos != null && pawn.Map.areaManager.Get<Area_Labor>() != null && !PrisonLaborUtility.WorkDisabled(workType))
                return pawn.Map.areaManager.Get<Area_Labor>()[pos];
            else
                return false;
        }
    }
}
