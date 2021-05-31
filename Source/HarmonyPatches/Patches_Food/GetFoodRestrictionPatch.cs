using HarmonyLib;
using PrisonLabor.Core.Needs;
using PrisonLabor.Core.Trackers;
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
    [HarmonyPatch(nameof(Pawn_FoodRestrictionTracker.GetCurrentRespectedRestriction))]
    class GetFoodRestrictionPatch
    {
        static FoodRestriction Postfix(FoodRestriction __result, Pawn_FoodRestrictionTracker __instance, Pawn getter)
        {
            if(__result == null && __instance.pawn.IsPrisonerOfColony && !__instance.pawn.InMentalState)
            {
                Need_Motivation motivation = __instance.pawn.needs.TryGetNeed<Need_Motivation>();
                if(motivation != null && (motivation.CurLevel > 0.7 || __instance.pawn.IsWatched() ))
                {
                    return __instance.CurrentFoodRestriction;
                }                
            }
            return __result;
        }
    }
}
