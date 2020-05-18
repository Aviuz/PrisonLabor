using HarmonyLib;
using PrisonLabor.Core;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_DeepDrill
{
    [HarmonyPatch(typeof(WorkGiver_DeepDrill))]
    static class EnableDeepDrillsToPrisoners
    {
        [HarmonyPostfix]
        [HarmonyPatch(nameof(WorkGiver_DeepDrill.HasJobOnThing))]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
        static bool HasJobOnThingPostfix(bool __result, WorkGiver_DeepDrill __instance, Pawn pawn, Thing t, bool forced)
        {
            Building building = t as Building;
            if (building != null && pawn != null && building.Faction.IsPlayer)
            {
                if (pawn.IsPrisonerOfColony)
                {
                    return true;
                }
                else if (pawn.Faction.IsPlayer)
                {
                    return !PrisonLaborUtility.IsDisabledByLabor(building.Position, pawn, __instance.def.workType);
                }                
            }
            return __result;
        }
    }
}
