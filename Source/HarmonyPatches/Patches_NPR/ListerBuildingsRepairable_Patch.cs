using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_NPR
{
    [HarmonyPatch(typeof(WorkGiver_Repair))]
    [HarmonyPatch("ShouldSkip")]
    [HarmonyPatch(new[] { typeof(Pawn), typeof(bool) })]
    class ListerBuildingsRepairable_Patch
    {
        
        static bool Prefix(ref bool __result, Pawn pawn, bool forced)
        {
            if(pawn.Faction == null && pawn.IsPrisonerOfColony)
            {
                __result = pawn.Map.listerBuildingsRepairable.RepairableBuildings(Faction.OfPlayer).Count == 0;
                return false;
            }
            return true;

        }
    }
}
