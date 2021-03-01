using HarmonyLib;
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
    [HarmonyPatch(typeof(WorkGiver_Repair))]
    class Patch_JobGiver_Repair
    {
        [HarmonyPrefix]
        [HarmonyPatch("ShouldSkip")]
        static bool shouldSkipPrefix(ref bool __result, Pawn pawn, bool forced)
        {
            if (pawn != null && pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer).Count == 0;
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("PotentialWorkThingsGlobal")]
        static bool PotentialWorkThingsGlobalPrefix(ref IEnumerable<Thing> __result, Pawn pawn)
        {
            if (pawn != null && pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer);
                return false;
            }

            return true;
        }

        [HarmonyPrefix]
        [HarmonyPatch("HasJobOnThing")]
        static bool HasJobOnThingPrefix(ref bool __result, Pawn pawn, Thing t, bool forced)
        {
            if(pawn != null && pawn.IsPrisonerOfColony)
            {
				Building building = t as Building;
                // Fuckin monster yeah I know. All negated values from oryginal method only for prisoners
				if (building != null && pawn.Map.listerBuildingsRepairable.Contains(Faction.OfPlayer, building)
					&& building.def.building.repairable && (t.def.useHitPoints || t.HitPoints != t.MaxHitPoints) && pawn.Map.areaManager.Home[t.Position]
					&& pawn.CanReserveAndReach(t, PathEndMode.ClosestTouch, pawn.NormalMaxDanger(), 1, -1, null, forced)
					&& building.Map.designationManager.DesignationOn(building, DesignationDefOf.Deconstruct) == null
					&& (!building.def.mineable && building.Map.designationManager.DesignationAt(building.Position, DesignationDefOf.Mine) == null)
					&& building.IsBurning()
				)
				{
                    __result = true;
                    return false;
				}
			}
            return true;
        }
    }
}
