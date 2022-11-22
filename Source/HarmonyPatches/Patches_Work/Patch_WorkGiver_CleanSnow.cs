using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;
using RimWorld;
using System.Reflection;
using PrisonLabor.Core;
using Verse.AI;

namespace PrisonLabor.HarmonyPatches.Patches_Work
{
  [HarmonyPatch(typeof(WorkGiver_ClearSnow), "HasJobOnCell")]
  class Patch_WorkGiver_CleanSnow
  {

    static bool Postfix(bool __result, Pawn pawn, IntVec3 c, bool forced)
    {
      if (__result && pawn.IsPrisonerOfColony)
      {
        return __result && pawn.CanReach(c, PathEndMode.OnCell, pawn.NormalMaxDanger());
      }
      return __result;
    }

  }
}
