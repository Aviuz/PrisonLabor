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

  [HarmonyPatch(typeof(WorkGiver_CarryToBuilding))]
  public class Patch_CarryToBuilding
  {
    [HarmonyPostfix]
    [HarmonyPatch("HasJobOnThing")]
    static bool HasJobOnThingPostfix(bool __result, Pawn pawn, Thing t, bool forced)
    {
      if (shouldChangeReturnValue(__result, pawn, t))
      {
        return false;
      }
      return __result;
    }

    [HarmonyPostfix]
    [HarmonyPatch("JobOnThing")]
    static Job JobOnThingPostfix(Job __result, Pawn pawn, Thing t, bool forced)
    {
      if (shouldChangeReturnValue(__result != null, pawn, t))
      {
        return null;
      }
      return __result;
    }

    private static bool shouldChangeReturnValue(bool result, Pawn pawn, Thing t)
    {
      return result && pawn.IsPrisonerOfColony && t is Building_Enterable building && building.SelectedPawn == pawn;
    }
  }
}
