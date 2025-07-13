using HarmonyLib;
using Verse;
using RimWorld;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(WorkGiver_ClearSnowOrSand), "HasJobOnCell")]
  class Patch_WorkGiver_CleanSnow
  {

    static bool Postfix(bool __result, Pawn pawn, IntVec3 c, bool forced)
    {
      if (__result && pawn.IsPrisonerOfColony)
      {
        return pawn.CanReach(c, PathEndMode.OnCell, pawn.NormalMaxDanger());
      }
      return __result;
    }

  }
}
