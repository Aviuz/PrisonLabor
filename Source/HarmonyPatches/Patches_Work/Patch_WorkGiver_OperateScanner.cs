using HarmonyLib;
using PrisonLabor.Core.Other;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
    [HarmonyPatch(typeof(WorkGiver_OperateScanner))]
    class Patch_WorkGiver_OperateScanner
    {
        [HarmonyPatch("ShouldSkip")]
        [HarmonyPostfix]
        static bool ShouldSkipPostfix(bool __result, WorkGiver_OperateScanner __instance, Pawn pawn, bool forced)
        {
            if (__result && pawn.IsPrisonerOfColony)
            {
                return CanOperate(pawn, __instance);
            }
            return __result;
        }

        private static bool CanOperate(Pawn pawn, WorkGiver_OperateScanner __instance)
        {
            List<Thing> list = pawn.Map.listerThings.ThingsOfDef(__instance.ScannerDef);
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].Faction == Faction.OfPlayer)
                {
                    CompScanner compScanner = list[i].TryGetComp<CompScanner>();
                    if (compScanner != null && compScanner.CanUseNow)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        [HarmonyPatch("HasJobOnThing")]
        [HarmonyPostfix]
        static bool HasJobOnThingPostfix(bool __result, Pawn pawn, Thing t, bool forced)
        {
            if(!__result && pawn.IsPrisonerOfColony)
            {
                if(t.Faction != Faction.OfPlayer)
                {
                    return false;
                }
                Building building = t as Building;
                if (building == null)
                {
                    return false;
                }
                if (building.IsForbidden(pawn))
                {
                    return false;
                }
                if (!pawn.CanReserve(building, 1, -1, null, forced))
                {
                    return false;
                }
                if (!building.TryGetComp<CompScanner>().CanUseNow)
                {
                    return false;
                }
                if (building.IsBurning())
                {
                    return false;
                }
                return true;
            }
            return __result;
        }
    }
}
