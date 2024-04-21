using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Food
{
    [HarmonyPatch(typeof(Pawn_FoodRestrictionTracker))]
    [HarmonyPatch("GetCurrentRespectedRestriction")]
    class Patch_EnableRespectingFoodPolicies
    {
    static FoodPolicy Postfix(FoodPolicy __result, Pawn_FoodRestrictionTracker __instance, Pawn getter)
        {
            if (__result == null)
            {
                if (__instance.pawn.IsPrisonerOfColony && __instance.pawn == getter)
                {
                    return __instance.CurrentFoodPolicy;
                }
            }
            return __result;
        }
    }
}
