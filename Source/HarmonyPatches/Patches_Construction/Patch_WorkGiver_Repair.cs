using HarmonyLib;
using PrisonLabor.WorkUtils;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch(typeof(WorkGiver_Repair))]
    class Patch_WorkGiver_Repair
    {
        [HarmonyPrefix]
        [HarmonyPatch("ShouldSkip")]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(bool) })]
        static bool ShouldSKipPrefix(ref bool __result, Pawn pawn, bool forced)
        {
            if(pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer).Count == 0;
                return false;
            }
            return true;

        }

        [HarmonyPrefix]
        [HarmonyPatch("PotentialWorkThingsGlobal")]
        [HarmonyPatch(new[] { typeof(Pawn) })]
        static bool PotentialWorkThingsGlobalPrefix(ref IEnumerable<Thing> __result, Pawn pawn)
        {
            if (pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer);
                return false;
            }
            return true;

        }


        [HarmonyPrefix]
        [HarmonyPatch("HasJobOnThing")]
        [HarmonyPatch(new[] { typeof(Pawn), typeof(Thing), typeof(bool) })]
        static bool HasJobOnThingPrefix(ref bool __result, Pawn pawn,Thing t, bool forced)
        {
            if (pawn.IsPrisonerOfColony)
            {
                __result = ConstructionUtils.HasJobOnThingFixed(pawn, t, forced);
                return false;
            }
            return true;

        }
    }
}
