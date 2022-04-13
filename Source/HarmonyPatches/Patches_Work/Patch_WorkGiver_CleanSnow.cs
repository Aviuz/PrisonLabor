using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    [HarmonyPatch(typeof(WorkGiver_ClearSnow), "HasJobOnCell")]
    class Patch_WorkGiver_CleanSnow
    {
        static bool Postfix(bool __result, Pawn pawn, IntVec3 c, bool forced)
        {
            if(pawn.IsPrisonerOfColony)
            {
                WorkGiverDef workGiverDef = DefDatabase<WorkGiverDef>.GetNamed("CleanClearSnow");
                return pawn.Map.snowGrid.GetDepth(c) >= 0.200000002980232 &&
                    !c.IsForbidden(pawn) &&
                    pawn.CanReserveAndReach(c, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced) &&
                    PrisonLaborUtility.CanWorkHere(c, pawn, workGiverDef.workType);
                
                    
            }
            return __result;
        }
    }
}
