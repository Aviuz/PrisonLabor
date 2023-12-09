using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch()]
    class Patch_Smoothing
    {
        static IEnumerable<MethodBase> TargetMethods()
        {
            yield return typeof(WorkGiver_ConstructSmoothWall).GetMethod("HasJobOnCell");
            yield return typeof(WorkGiver_ConstructAffectFloor).GetMethod("HasJobOnCell");
        }

        public static bool Postfix(bool __result, Pawn pawn, IntVec3 c)
        {
            if(__result && pawn.IsPrisonerOfColony)
            {
               return pawn.CanReach(c, PathEndMode.Touch, pawn.NormalMaxDanger());
            }

            return __result;
        }
    }
}
