using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.Reflection;
using PrisonLabor.Core;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    [HarmonyPatch]
    class Patch_WorkGiver_CleanFilth
    {
        private static int MinTicksSinceThickened = 600;
        static MethodBase TargetMethod()
        {
            Assembly asm = typeof(WorkGiver_Scanner).Assembly;
            Type type = asm.GetType("RimWorld.WorkGiver_CleanFilth");          
            return type.GetMethod("HasJobOnThing");
        }
        static bool Postfix(bool __result,  Pawn pawn, Thing t, bool forced)
        {
            if (!__result)
            {
                if (pawn.Faction != Faction.OfPlayer && !pawn.IsPrisonerOfColony)
                {
                    return __result;
                }

                WorkGiverDef workGiverDef = DefDatabase<WorkGiverDef>.GetNamed("CleanFilth");
                return t is Filth filth && filth.Map.areaManager.Home[filth.Position]
                    && pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced)
                    && filth.TicksSinceThickened >= MinTicksSinceThickened
                    && PrisonLaborUtility.canWorkHere(filth.Position, pawn, workGiverDef.workType);
            }
            return __result;
        }
        
    }
}
