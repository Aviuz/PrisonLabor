using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace PrisonLabor.HarmonyPatches.Patches_Construction
{
    [HarmonyPatch()]
    [HarmonyPatch(typeof(WorkGiver_BuildRoof), "HasJobOnCell")]
    class Patch_RoofReservation
    {
        public static bool Postfix(bool __result, WorkGiver_BuildRoof __instance, Pawn pawn, IntVec3 c, bool forced)
        {
            if (__result && pawn.IsPrisonerOfColony)
            {
                try
                {
                    System.Reflection.MethodInfo methodInfo = AccessTools.Method(typeof(WorkGiver_BuildRoof), "BuildingToTouchToBeAbleToBuildRoof");
                    Building building = methodInfo.Invoke(__instance, new object[] { c, pawn }) as Building;
                    if (building != null)
                    {
                        return pawn.CanReach(building, Verse.AI.PathEndMode.Touch, pawn.NormalMaxDanger());
                    }
                } catch(Exception e)
                {
                    Verse.Log.Message($"Exception in roof patch{e}");
                }
            }
            return __result;
        }
    }
}
